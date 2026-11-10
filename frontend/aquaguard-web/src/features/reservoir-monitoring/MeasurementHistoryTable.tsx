import type { LevelMeasurement } from '../../services/reservoirMonitoring'

type Props = { measurements: LevelMeasurement[] }

export function MeasurementHistoryTable({ measurements }: Props) {
  return (
    <div className="table-scroll">
      <table>
        <caption>Leituras de nível simuladas no período selecionado</caption>
        <thead><tr><th scope="col">Horário local</th><th scope="col">Nível</th><th scope="col">Origem</th></tr></thead>
        <tbody>
          {measurements.length === 0 ? (
            <tr><td colSpan={3}>Sem medições neste período.</td></tr>
          ) : measurements.map((measurement, index) => (
            <tr key={`${measurement.measuredAt}-${index}`}>
              <td>{formatTimestamp(measurement.measuredAt)}</td>
              <td>{measurement.value.toFixed(2)} m</td>
              <td>Simulada</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}

function formatTimestamp(value: string) {
  return new Intl.DateTimeFormat('pt-BR', { dateStyle: 'short', timeStyle: 'short' }).format(new Date(value))
}
