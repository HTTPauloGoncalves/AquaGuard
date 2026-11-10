import type { LevelMeasurement } from '../../services/reservoirMonitoring'

type Props = { measurements: LevelMeasurement[] }

export function LevelHistoryChart({ measurements }: Props) {
  if (measurements.length === 0) {
    return <p className="empty-state">Não há medições válidas de nível neste período.</p>
  }

  const width = 760
  const height = 256
  const padding = { top: 18, right: 24, bottom: 42, left: 66 }
  const values = measurements.map((point) => point.value)
  const minValue = Math.min(...values)
  const maxValue = Math.max(...values)
  const visualPadding = Math.max((maxValue - minValue) * 0.12, Math.abs(maxValue) * 0.02, 0.05)
  const domainMin = minValue - visualPadding
  const domainMax = maxValue + visualPadding
  const times = measurements.map((point) => new Date(point.measuredAt).getTime())
  const firstTime = Math.min(...times)
  const timeSpan = Math.max(...times) - firstTime
  const graphWidth = width - padding.left - padding.right
  const graphHeight = height - padding.top - padding.bottom
  const points = measurements.map((point, index) => ({
    x: timeSpan === 0 ? width / 2 : padding.left + ((times[index] - firstTime) / timeSpan) * graphWidth,
    y: padding.top + ((domainMax - point.value) / (domainMax - domainMin)) * graphHeight,
    measurement: point,
  }))
  const ticks = Array.from({ length: 4 }, (_, index) => domainMax - ((domainMax - domainMin) * index) / 3)
  const summary = `Gráfico de pontos com ${measurements.length} medições simuladas, em metros. Menor valor ${formatLevel(minValue)} m; maior valor ${formatLevel(maxValue)} m. Os pontos representam apenas horários medidos, sem interpolação.`

  return (
    <>
      <div className="chart-frame">
        <svg aria-describedby="level-chart-summary" aria-label="Medições de nível no período, em metros" className="level-chart" role="img" viewBox={`0 0 ${width} ${height}`}>
          <title>Histórico simulado do nível</title>
          <desc>{summary}</desc>
          {ticks.map((value, index) => {
            const y = padding.top + (graphHeight * index) / 3
            return (
              <g className="chart-grid-row" key={value}>
                <line className="chart-gridline" x1={padding.left} x2={width - padding.right} y1={y} y2={y} />
                <text className="chart-label" textAnchor="end" x={padding.left - 12} y={y + 4}>{formatLevel(value)} m</text>
              </g>
            )
          })}
          <line className="chart-axis" x1={padding.left} x2={width - padding.right} y1={height - padding.bottom} y2={height - padding.bottom} />
          {points.map(({ x, y, measurement }, index) => (
            <circle className="chart-point" cx={x} cy={y} key={`${measurement.measuredAt}-${index}`} r="5">
              <title>{formatLevel(measurement.value)} m · {formatTimestamp(measurement.measuredAt)} · Simulada</title>
            </circle>
          ))}
          <text className="chart-time-label" textAnchor="start" x={padding.left} y={height - 12}>{formatTimestamp(measurements[0].measuredAt)}</text>
          {measurements.length > 1 && (
            <text className="chart-time-label" textAnchor="end" x={width - padding.right} y={height - 12}>
              {formatTimestamp(measurements[measurements.length - 1].measuredAt)}
            </text>
          )}
        </svg>
      </div>
      <p className="chart-note" id="level-chart-summary">Cada ponto representa uma medição recebida. Intervalos sem leitura permanecem sem preenchimento.</p>
    </>
  )
}

function formatLevel(value: number) {
  return new Intl.NumberFormat('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 }).format(value)
}

function formatTimestamp(value: string) {
  return new Intl.DateTimeFormat('pt-BR', { dateStyle: 'short', timeStyle: 'short' }).format(new Date(value))
}
