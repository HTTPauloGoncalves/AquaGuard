export type LevelMeasurement = {
  value: number
  unit: 'm'
  measuredAt: string
  source: 'simulated'
}

export type PrecipitationMeasurement = {
  value: number
  unit: 'mm'
  measuredAt: string
  accumulationStart: string
  accumulationEnd: string
  source: 'simulated'
}

export type LevelVariation = {
  deltaMeters: number
  startMeasuredAt: string
  endMeasuredAt: string
  direction: 'rising' | 'falling' | 'no_observed_change'
}

export type MonitoringResponse = {
  reservoirId: string
  reservoirName: string
  levelReferenceLabel: string
  window: { from: string; to: string }
  latestValid: LevelMeasurement | null
  history: LevelMeasurement[]
  variation: LevelVariation | null
  precipitation: PrecipitationMeasurement[] | null
}

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5226/api/v1'

export async function getReservoirMonitoring(
  reservoirId: string,
  from?: string,
  to?: string,
  includePrecipitation = false,
  signal?: AbortSignal,
): Promise<MonitoringResponse> {
  const query = new URLSearchParams({ includePrecipitation: String(includePrecipitation) })
  if (from) query.set('from', from)
  if (to) query.set('to', to)

  let response: Response
  try {
    response = await fetch(`${apiBaseUrl}/reservoirs/${reservoirId}/level-monitoring?${query}`, { signal })
  } catch (error) {
    if (error instanceof DOMException && error.name === 'AbortError') throw error
    throw new Error('Não foi possível conectar à API. Verifique se o ambiente está disponível.', { cause: error })
  }
  if (!response.ok) throw new Error(`A API não conseguiu carregar o monitoramento (HTTP ${response.status}).`)
  return (await response.json()) as MonitoringResponse
}
