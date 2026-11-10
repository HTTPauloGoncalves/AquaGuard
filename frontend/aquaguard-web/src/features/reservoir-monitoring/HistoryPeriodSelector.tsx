export type PeriodPreset = '1h' | '6h' | '24h' | '7d' | 'custom'

type Props = {
  value: PeriodPreset
  onChange: (value: PeriodPreset) => void
  from: string
  to: string
  onCustomChange: (from: string, to: string) => void
}

const presets: { id: PeriodPreset; label: string }[] = [
  { id: '1h', label: '1 hora' },
  { id: '6h', label: '6 horas' },
  { id: '24h', label: '24 horas' },
  { id: '7d', label: '7 dias' },
  { id: 'custom', label: 'Personalizado' },
]

export function HistoryPeriodSelector({ value, onChange, from, to, onCustomChange }: Props) {
  return (
    <div className="period-control">
      <fieldset>
        <legend>Período do histórico</legend>
        <div className="period-options">
          {presets.map((preset) => (
            <button
              aria-pressed={value === preset.id}
              className={value === preset.id ? 'period-button is-selected' : 'period-button'}
              key={preset.id}
              onClick={() => onChange(preset.id)}
              type="button"
            >
              {preset.label}
            </button>
          ))}
        </div>
      </fieldset>
      {value === 'custom' && (
        <div className="custom-range">
          <label>
            Início
            <input aria-label="Início do período" onChange={(event) => onCustomChange(event.target.value, to)} type="datetime-local" value={from} />
          </label>
          <label>
            Fim
            <input aria-label="Fim do período" onChange={(event) => onCustomChange(from, event.target.value)} type="datetime-local" value={to} />
          </label>
        </div>
      )}
    </div>
  )
}
