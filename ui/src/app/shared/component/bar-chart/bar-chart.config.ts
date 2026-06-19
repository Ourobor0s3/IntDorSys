import { ChartData } from '../../interface/chartData.model';

type ChartTheme = {
    text: string;
    muted: string;
    grid: string;
    tooltipBg: string;
    tooltipBorder: string;
};

type ChartLabels = {
    label1: string;
    label2: string;
};

export class ChartConfigFactory {
    static chartTheme(currentTheme: 'light' | 'dark'): ChartTheme {
        const isDark = currentTheme === 'dark';
        return {
            text: isDark ? '#e4e4e7' : '#374151',
            muted: isDark ? '#9ca3af' : '#6b7280',
            grid: isDark ? '#ffffff0d' : '#0000000a',
            tooltipBg: isDark ? '#1e1e2d' : '#ffffff',
            tooltipBorder: isDark ? '#2a2a3d' : '#e5e7eb',
        };
    }

    static buildChartData(chartInfo: ChartData[], ctx: CanvasRenderingContext2D, labels: ChartLabels) {
        return {
            labels: chartInfo.map(data => data.name),
            datasets: [
                {
                    label: labels.label1,
                    data: chartInfo.map(data => data.value1),
                    backgroundColor: buildGradient(ctx, '#6366f1', '#a5b4fc'),
                    borderRadius: 6,
                    borderSkipped: false,
                    barPercentage: 0.6,
                },
                {
                    label: labels.label2,
                    data: chartInfo.map(data => data.value2),
                    backgroundColor: buildGradient(ctx, '#22c55e', '#86efac'),
                    borderRadius: 6,
                    borderSkipped: false,
                    barPercentage: 0.6,
                },
            ],
        };
    }

    static buildChartOptions(theme: ChartTheme) {
        return {
            responsive: true,
            maintainAspectRatio: true,
            aspectRatio: 1.8,
            animation: {
                duration: 800,
                easing: 'easeOutQuart' as const,
            },
            plugins: {
                legend: {
                    display: true,
                    position: 'top' as const,
                    labels: {
                        color: theme.text,
                        boxWidth: 12,
                        boxHeight: 12,
                        usePointStyle: true,
                        pointStyle: 'rectRounded' as const,
                        padding: 16,
                    },
                },
                tooltip: {
                    backgroundColor: theme.tooltipBg,
                    titleColor: theme.text,
                    bodyColor: theme.muted,
                    borderColor: theme.tooltipBorder,
                    borderWidth: 1,
                    cornerRadius: 8,
                    padding: 10,
                    boxPadding: 4,
                },
            },
            scales: {
                x: {
                    ticks: {
                        color: theme.muted,
                        font: {
                            size: 11,
                        },
                    },
                    grid: {
                        display: false,
                    },
                },
                y: {
                    beginAtZero: true,
                    ticks: {
                        color: theme.muted,
                        font: {
                            size: 11,
                        },
                        precision: 0,
                    },
                    grid: {
                        color: theme.grid,
                    },
                },
            },
        };
    }
}

function buildGradient(ctx: CanvasRenderingContext2D, color1: string, color2: string) {
    const gradient = ctx.createLinearGradient(0, 0, 0, ctx.canvas.height * 0.7);
    gradient.addColorStop(0, color1);
    gradient.addColorStop(1, color2);
    return gradient;
}
