import json
from datetime import datetime as dt

DATA = json.load(open("gantt_data.json"))
PROJECT, SPRINTS, HOLIDAYS, MILESTONES, TASKS = DATA["project"], DATA["sprints"], DATA["holidays"], DATA["milestones"], DATA["tasks"]
TODAY = PROJECT["today"]

def D(s): return dt.strptime(s, "%Y-%m-%d")

STATUS_COLOURS = {
    "done":        ("#22c55e", "#22c55e"),
    "in_progress": ("#bae6fd", "#0ea5e9"),
    "not_started": ("#c2ccd8", "#94a3b8"),
}
BORDER_COLOURS = {"not_started": "#8fa0b3"}
LABELS = {"done": "Complete", "in_progress": "In progress", "not_started": "Not started"}
MILESTONE_COLOUR = "#6d28d9"

def progress(t, st):
    if st == "done": return 1.0
    if st == "not_started": return 0.0
    s, e, n = D(t["start"]), D(t["end"]), D(TODAY)
    if e <= s: return 0.5
    if n <= s: return 0.0
    if n >= e: return 1.0
    return (n - s) / (e - s)

def status(t):
    n = D(TODAY)
    if D(t["end"]) <= n: return "done"
    if D(t["start"]) > n: return "not_started"
    return "in_progress"

import plotly.graph_objects as go
fig = go.Figure()

rows = list(reversed(TASKS))
ylabels = [t["name"] for t in rows]

# Sprints & Breaks
segs = [dict(name=sp["name"], start=sp["start"], end=sp["end"], si=i) for i, sp in enumerate(SPRINTS)]
segs += [dict(name="Break", start=h["start"], end=h["end"], si=None) for h in HOLIDAYS]
segs.sort(key=lambda s: s["start"])
for s in segs:
    brk = s["si"] is None
    fill = "#d3dae2" if brk else ("#e8f0fa" if s["si"] % 2 else "#f4f8fc")
    fig.add_vrect(x0=D(s["start"]), x1=D(s["end"]), fillcolor=fill, layer="below", line_width=0,
                  annotation_text=s["name"], annotation_position="top left",
                  annotation_font_size=11, annotation_font_color=("#54606e" if brk else "#33475b"))

for bnd in sorted({s["start"] for s in segs} | {s["end"] for s in segs}):
    fig.add_vline(x=D(bnd), line=dict(color="#8b98a8", width=1.4), opacity=0.8, layer="below")

def bar(y, x0, x1, color, width, opacity, hover, group, border=None):
    x0d = D(x0)
    ms = (D(x1) - x0d).total_seconds() * 1000
    if ms <= 0:
        ms = 0.6 * 86400 * 1000
    line = dict(color=border, width=1.2) if border else dict(width=0)
    fig.add_trace(go.Bar(base=[x0d], x=[ms], y=[y], orientation="h", width=width,
                         marker=dict(color=color, line=line), opacity=opacity,
                         legendgroup=group, showlegend=False,
                         hovertemplate=hover + "<extra></extra>"))

for t in rows:
    y = t["name"]
    st = status(t)
    base, solid = STATUS_COLOURS[st]
    rng = t["start"] if t["start"] == t["end"] else f"{t['start']} \u2192 {t['end']}"
    hov = f"<b>{t['name']}</b><br>{LABELS[st]}<br>{rng}"
    bar(y, t["start"], t["end"], base, 0.55, 1.0, hov, st, border=BORDER_COLOURS.get(st))
    fr = progress(t, st)
    if 0 < fr < 1:
        endp = D(t["start"]) + (D(t["end"]) - D(t["start"])) * fr
        bar(y, t["start"], endp.strftime("%Y-%m-%d"), solid, 0.55, 1.0, hov, st)

# Milestones
for m in MILESTONES:
    x = D(m["date"])
    fig.add_vline(x=x, line=dict(color=MILESTONE_COLOUR, width=2, dash="dash"), opacity=0.9)
    fig.add_annotation(x=x, y=1.008, yref="paper", showarrow=False,
                       text="\u25c6", font=dict(size=17, color=MILESTONE_COLOUR))
    fig.add_annotation(x=x, y=1.075, yref="paper", showarrow=False,
                       text=f"<b>{m['name']}</b>", font=dict(size=10.5, color="#ffffff"),
                       align="center", bgcolor=MILESTONE_COLOUR, borderpad=4, bordercolor=MILESTONE_COLOUR)

# Today
fig.add_vline(x=D(TODAY), line=dict(color="#ef4444", width=2))
fig.add_annotation(x=D(TODAY), y=1.045, yref="paper", showarrow=False, text="<b>TODAY</b>",
                   font=dict(size=10, color="#fff"), bgcolor="#ef4444", borderpad=3)

# Legend
for st in ["done", "in_progress", "not_started"]:
    fig.add_trace(go.Bar(x=[None], y=[None], marker_color=STATUS_COLOURS[st][1], name=LABELS[st],
                         legendgroup=st, showlegend=True))

fig.update_layout(
    title=dict(text=f"<b>{PROJECT['name']}</b>  \u00b7  Gantt Chart", font=dict(size=22), y=0.97),
    barmode="overlay", bargap=0.35, height=580, plot_bgcolor="white",
    font=dict(family="Inter, Segoe UI, sans-serif", color="#0e1b2a"),
    margin=dict(l=190, r=30, t=96, b=64),
    legend=dict(orientation="h", y=-0.12, x=0, groupclick="togglegroup"),
    xaxis=dict(type="date", showgrid=False,
               range=[D(PROJECT["range_start"]), D(PROJECT["range_end"])]),
    yaxis=dict(categoryorder="array", categoryarray=ylabels, showgrid=False, tickfont=dict(size=11)),
)

FILE_NAME = "gantt.html"
fig.write_html(FILE_NAME, include_plotlyjs=True)
print(f"Saved to {FILE_NAME}")
