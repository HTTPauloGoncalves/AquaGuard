import type { PrecipitationMeasurement } from '../../services/reservoirMonitoring'

type Props = { measurements: PrecipitationMeasurement[] | null; timeZone: string }

export function PrecipitationContext({ measurements, timeZone }: Props) {
  if (measurements === null) return <p className="muted-note">Série de precipitação não solicitada.</p>
  if (measurements.length === 0) return <p className="empty-state">Sem precipitação registrada neste período; isso não significa chuva igual a zero.</p>

  const max = Math.max(...measurements.map((measurement) => measurement.value), 1)

  return (
    <>
      <svg aria-label="Acumulações de precipitação simulada em milímetros" className="rain-chart" role="img" viewBox={`0 0 ${Math.max(measurements.length * 72, 320)} 140`}>
        {measurements.map((measurement, index) => {
          const x = index * 72 + 28
          const barHeight = measurement.value === 0 ? 2 : Math.max(4, (measurement.value / max) * 92)
          return (
            <g key={`${measurement.accumulationStart}-${measurement.accumulationEnd}`}>
              <rect className={measurement.value === 0 ? 'rain-bar is-zero' : 'rain-bar'} height={barHeight} rx="4" width="30" x={x} y={106 - barHeight} />
              <text className="rain-value" textAnchor="middle" x={x + 15} y={96 - barHeight}>{measurement.value.toFixed(1)}</text>
              <text className="rain-tick" textAnchor="middle" x={x + 15} y="126">{formatShortDate(measurement.accumulationEnd)}</text>
            </g>
          )
        })}
        <line className="chart-axis" x1="12" x2={Math.max(measurements.length * 72 - 12, 308)} y1="107" y2="107" />
      </svg>
      <p className="sr-only">Valores expressos em milímetros; uma barra vazia representa precipitação medida igual a zero.</p>
    <div className="table-scroll">
      <table>
        <caption>Precipitação simulada e intervalos de acumulação · {timeZone}</caption>
        <thead><tr><th scope="col">Acumulação</th><th scope="col">Precipitação</th><th scope="col">Origem</th></tr></thead>
        <tbody>{measurements.map((measurement) => (
          <tr key={`${measurement.accumulationStart}-${measurement.accumulationEnd}`}>
            <td>{formatTimestamp(measurement.accumulationStart)} – {formatTimestamp(measurement.accumulationEnd)}</td>
            <td>{measurement.value.toFixed(1)} mm</td>
            <td>Simulada</td>
          </tr>
        ))}</tbody>
      </table>
    </div>
    </>
  )
}

function formatTimestamp(value: string) {
  return new Intl.DateTimeFormat('pt-BR', { dateStyle: 'short', timeStyle: 'short' }).format(new Date(value))
}

function formatShortDate(value: string) {
  return new Intl.DateTimeFormat('pt-BR', { day: '2-digit', month: '2-digit' }).format(new Date(value))
}
