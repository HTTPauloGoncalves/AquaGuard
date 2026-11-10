import { useEffect, useMemo, useState } from 'react'
import { getReservoirMonitoring, type LevelMeasurement, type MonitoringResponse } from '../../services/reservoirMonitoring'
import { CurrentLevelPanel } from './CurrentLevelPanel'
import { HistoryPeriodSelector, type PeriodPreset } from './HistoryPeriodSelector'
import { LevelHistoryChart } from './LevelHistoryChart'
import { MeasurementHistoryTable } from './MeasurementHistoryTable'
import { PrecipitationContext } from './PrecipitationContext'

const reservoirId = import.meta.env.VITE_RESERVOIR_ID ?? '00000000-0000-0000-0000-000000000001'
const periodMilliseconds: Record<Exclude<PeriodPreset, 'custom'>, number> = {
  '1h': 60 * 60 * 1000,
  '6h': 6 * 60 * 60 * 1000,
  '24h': 24 * 60 * 60 * 1000,
  '7d': 7 * 24 * 60 * 60 * 1000,
}

type QueryResult = {
  key: string
  response: MonitoringResponse
  fetchedAt: Date
}

type QueryError = { key: string; message: string }

export function ReservoirMonitoringPage() {
  const [period, setPeriod] = useState<PeriodPreset>('24h')
  const [customFrom, setCustomFrom] = useState('')
  const [customTo, setCustomTo] = useState('')
  const [includePrecipitation, setIncludePrecipitation] = useState(false)
  const [queryResult, setQueryResult] = useState<QueryResult | null>(null)
  const [latestLevel, setLatestLevel] = useState<LevelMeasurement | null>(null)
  const [queryError, setQueryError] = useState<QueryError | null>(null)
  const [loading, setLoading] = useState(true)
  const [refreshCounter, setRefreshCounter] = useState(0)

  const customRangeStatus = useMemo(() => {
    if (!customFrom || !customTo) return 'incomplete'
    const fromTime = new Date(customFrom).getTime()
    const toTime = new Date(customTo).getTime()
    if (!Number.isFinite(fromTime) || !Number.isFinite(toTime) || fromTime > toTime) return 'invalid'
    return 'ready'
  }, [customFrom, customTo])

  const selectedWindow = useMemo(() => {
    if (period === 'custom') {
      if (customRangeStatus !== 'ready') return { from: undefined, to: undefined }
      return { from: new Date(customFrom).toISOString(), to: new Date(customTo).toISOString() }
    }
    const end = new Date()
    return { from: new Date(end.getTime() - periodMilliseconds[period]).toISOString(), to: end.toISOString() }
  }, [period, customFrom, customTo, customRangeStatus, refreshCounter])

  const queryKey = period === 'custom'
    ? `${period}:${customFrom}:${customTo}:${includePrecipitation}`
    : `${period}:${includePrecipitation}`
  const visibleResult = queryResult?.key === queryKey ? queryResult : null
  const visibleError = queryError?.key === queryKey ? queryError.message : null

  useEffect(() => {
    const timer = window.setInterval(() => setRefreshCounter((current) => current + 1), 15_000)
    return () => window.clearInterval(timer)
  }, [])

  useEffect(() => {
    const controller = new AbortController()
    if (period === 'custom' && customRangeStatus !== 'ready') {
      setLoading(false)
      setQueryError(null)
      return () => controller.abort()
    }

    setLoading(true)
    setQueryError(null)
    getReservoirMonitoring(reservoirId, selectedWindow.from, selectedWindow.to,
      includePrecipitation, controller.signal)
      .then((response) => {
        setQueryResult({ key: queryKey, response, fetchedAt: new Date() })
        setLatestLevel(response.latestValid)
      })
      .catch((reason: unknown) => {
        if (reason instanceof DOMException && reason.name === 'AbortError') return
        setQueryError({
          key: queryKey,
          message: reason instanceof Error ? reason.message : 'Ocorreu um erro ao carregar as medições.',
        })
      })
      .finally(() => {
        if (!controller.signal.aborted) setLoading(false)
      })

    return () => controller.abort()
  }, [selectedWindow, includePrecipitation, queryKey, period, customRangeStatus])

  const timeZone = Intl.DateTimeFormat().resolvedOptions().timeZone
  const lastMeasuredAt = latestLevel?.measuredAt
  const updateLabel = loading
    ? 'Consultando dados'
    : visibleError
      ? 'Falha na consulta'
      : visibleResult
        ? `Atualizado às ${formatTime(visibleResult.fetchedAt)}`
        : period === 'custom' && customRangeStatus !== 'ready'
          ? 'Defina o período'
          : 'Aguardando dados'

  return (
    <main className="app-shell">
      <header className="topbar">
        <a aria-label="AquaGuard, início" className="brand" href="#inicio">
          <span aria-hidden="true" className="brand-mark">A</span>
          <span>AquaGuard<small>Monitoramento hídrico</small></span>
        </a>
        <div className="environment-badge"><span aria-hidden="true" /> Ambiente de demonstração</div>
      </header>

      <div className="content" id="inicio">
        <section className="page-heading">
          <div className="heading-copy">
            <p className="eyebrow">Painel operacional <span aria-hidden="true">/</span> Reservatório</p>
            <h1>{latestLevel ? visibleResult?.response.reservoirName ?? queryResult?.response.reservoirName : 'Açude monitorado'}</h1>
            <p className="subtitle">Leituras simuladas para acompanhar o nível e suas mudanças observadas.</p>
          </div>
          <div className="heading-actions">
            <div aria-live="polite" className="update-status" role="status">
              <span aria-hidden="true" className={loading ? 'status-dot is-loading' : visibleError ? 'status-dot is-error' : 'status-dot'} />
              <span>{updateLabel}</span>
            </div>
            <button aria-label="Atualizar monitoramento agora" className="refresh-button" disabled={loading} onClick={() => setRefreshCounter((current) => current + 1)} type="button">
              <span aria-hidden="true" className="refresh-icon">↻</span>
              Atualizar
            </button>
          </div>
        </section>

        {visibleError && (
          <div className="error-banner" role="alert">
            <strong>Não foi possível atualizar este período.</strong>
            <span>{visibleError}</span>
            {visibleResult && <small>O resultado mostrado abaixo é da consulta anterior, feita às {formatTime(visibleResult.fetchedAt)}.</small>}
          </div>
        )}

        <CurrentLevelPanel latest={latestLevel} loading={!latestLevel && loading} referenceLabel={queryResult?.response.levelReferenceLabel ?? 'Referencial sintético'} timeZone={timeZone} variation={visibleResult?.response.variation ?? null} />

        {lastMeasuredAt && (
          <p className="stale-note">Última medição registrada em {formatTimestamp(lastMeasuredAt)} ({timeZone}). Este horário é da medição, não da consulta.</p>
        )}

        <div className="monitoring-grid">
          <section aria-labelledby="history-title" className="card history-card">
            <div className="section-heading">
              <div>
                <p className="eyebrow">Série temporal</p>
                <h2 id="history-title">Histórico do nível</h2>
              </div>
              <span className="timezone-label">Horários em {timeZone}</span>
            </div>

            <HistoryPeriodSelector value={period} onChange={setPeriod} from={customFrom} to={customTo}
              onCustomChange={(from, to) => { setCustomFrom(from); setCustomTo(to) }} />

            {period === 'custom' && customRangeStatus === 'incomplete' && (
              <p className="range-message" role="status">Informe início e fim para consultar o período. Os dados de outro intervalo foram ocultados.</p>
            )}
            {period === 'custom' && customRangeStatus === 'invalid' && (
              <p className="range-message is-error" role="alert">O início deve ser anterior ou igual ao fim do período.</p>
            )}

            {visibleResult ? (
              <>
                <LevelHistoryChart measurements={visibleResult.response.history} />
                <MeasurementHistoryTable measurements={visibleResult.response.history} />
              </>
            ) : period === 'custom' && customRangeStatus !== 'ready' ? null : (
              <p className="empty-state" role="status">
                {loading ? 'Carregando o histórico deste período…' : visibleError ? 'Histórico indisponível para esta consulta.' : 'Aguardando medições deste período.'}
              </p>
            )}
          </section>

          <section aria-labelledby="weather-title" className="card weather-card">
            <div className="section-heading">
              <div>
                <p className="eyebrow">Contexto meteorológico</p>
                <h2 id="weather-title">Precipitação</h2>
              </div>
              <label className="toggle-control">
                <input checked={includePrecipitation} onChange={(event) => setIncludePrecipitation(event.target.checked)} type="checkbox" />
                <span>Mostrar série</span>
              </label>
            </div>
            {!includePrecipitation ? (
              <p className="muted-note">Consulte a precipitação como uma série separada. Coincidência temporal não demonstra que a chuva causou uma variação do nível.</p>
            ) : visibleResult ? (
              <PrecipitationContext measurements={visibleResult.response.precipitation} timeZone={timeZone} />
            ) : period === 'custom' && customRangeStatus !== 'ready' ? (
              <p className="muted-note">Defina um intervalo válido para consultar a precipitação.</p>
            ) : (
              <p className="empty-state" role="status">{loading ? 'Carregando a precipitação deste período…' : visibleError ? 'Precipitação indisponível para esta consulta.' : 'Aguardando dados de precipitação.'}</p>
            )}
          </section>
        </div>

        <footer className="page-footer">
          <span>Todos os dados desta demonstração são sintéticos; não representam observações físicas do reservatório.</span>
          {lastMeasuredAt && <span>Última leitura: {formatTimestamp(lastMeasuredAt)}</span>}
        </footer>
      </div>
    </main>
  )
}

function formatTimestamp(value: string | Date) {
  return new Intl.DateTimeFormat('pt-BR', { dateStyle: 'short', timeStyle: 'short' }).format(new Date(value))
}

function formatTime(value: Date) {
  return new Intl.DateTimeFormat('pt-BR', { timeStyle: 'short' }).format(value)
}
