import type { LevelMeasurement, LevelVariation } from '../../services/reservoirMonitoring'

type Props = {
  latest: LevelMeasurement | null
  loading: boolean
  variation: LevelVariation | null
  referenceLabel: string
  timeZone: string
}

export function CurrentLevelPanel({ latest, loading, variation, referenceLabel, timeZone }: Props) {
  const direction = variation?.direction === 'rising' ? 'Elevação observada'
    : variation?.direction === 'falling' ? 'Redução observada'
      : variation ? 'Sem variação observada entre os pontos' : 'Variação indisponível'

  return (
    <section aria-labelledby="current-level-title" className="current-level card">
      <div className="current-reading">
        <div className="reading-heading">
          <div>
            <p className="eyebrow">Leitura mais recente</p>
            <h2 id="current-level-title">Nível do reservatório</h2>
          </div>
          <span className="simulated-pill"><span aria-hidden="true" /> Dado simulado</span>
        </div>
        {latest ? (
          <>
            <p aria-label={`${formatLevel(latest.value)} metros`} className="level-value">{formatLevel(latest.value)} <span>m</span></p>
            <p className="measurement-time">Medido em <time dateTime={latest.measuredAt}>{formatTimestamp(latest.measuredAt)}</time> · {timeZone}</p>
            <p className="reference-note">Referencial: {referenceLabel}</p>
          </>
        ) : loading ? (
          <p className="empty-state" role="status">Consultando a última medição válida…</p>
        ) : (
          <p className="empty-state">Ainda não há medições válidas de nível.</p>
        )}
      </div>
      <div aria-live="polite" className={`variation-card${variation?.direction === 'falling' ? ' is-falling' : ''}`}>
        <span className="eyebrow">Variação no intervalo</span>
        <strong>{variation ? `${variation.deltaMeters > 0 ? '+' : ''}${formatLevel(variation.deltaMeters)} m` : '—'}</strong>
        <span className="variation-direction">{direction}</span>
        {variation && (
          <small>
            De {formatTimestamp(variation.startMeasuredAt)} a {formatTimestamp(variation.endMeasuredAt)}
          </small>
        )}
        <span className="variation-method">Δh = nível final − nível inicial</span>
      </div>
    </section>
  )
}

function formatLevel(value: number) {
  return new Intl.NumberFormat('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 }).format(value)
}

function formatTimestamp(value: string) {
  return new Intl.DateTimeFormat('pt-BR', { dateStyle: 'short', timeStyle: 'short' }).format(new Date(value))
}
