import { Component, ElementRef, Input, OnDestroy, OnInit, SimpleChanges, ViewChild } from '@angular/core';
import { Chart, registerables } from "chart.js";
import { ChartData } from "../../model/chartData.model";
import { TranslateService } from '@ngx-translate/core';
import { EventService } from '../../services/event.service';
import { ThemeService } from '../../services/theme.service';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
    selector: 'app-bar-chart',
    templateUrl: './bar-chart.component.html',
    styleUrls: ['./bar-chart.component.scss'],
})
export class BarChartComponent implements OnInit, OnDestroy {
    @ViewChild('customChart', { static: false }) canvasRef!: ElementRef<HTMLCanvasElement>;
    @Input() public chartInfo: ChartData[];

    public chart: Chart | null;
    private currentTheme: 'light' | 'dark' = 'light';
    private destroy$ = new Subject<void>();

    constructor(
        private translate: TranslateService,
        private eventService: EventService,
        private themeService: ThemeService,
    ) {
    }

    ngOnInit(): void {
        this.currentTheme = this.themeService.getTheme();
        Chart.register(...registerables);
        this.createChart();

        this.eventService.LangChangeEvent
            .pipe(takeUntil(this.destroy$))
            .subscribe(() => {
                this.updateChartLabels();
            });

        this.translate.onLangChange
            .pipe(takeUntil(this.destroy$))
            .subscribe(() => {
                this.updateChartLabels();
            });

        this.themeService.theme$
            .pipe(takeUntil(this.destroy$))
            .subscribe((theme) => {
                this.currentTheme = theme;
                this.updateChartTheme();
            });
    }

    ngOnChanges(changes: SimpleChanges): void {
        if (changes['chartInfo'] && this.chartInfo && this.chartInfo.length > 0) {
            this.createChart();
        }
    }

    ngOnDestroy(): void {
        this.destroy$.next();
        this.destroy$.complete();
    }

    private chartTheme() {
        const isDark = this.currentTheme === 'dark';
        return {
            text: isDark ? '#e4e4e7' : '#374151',
            muted: isDark ? '#9ca3af' : '#6b7280',
            grid: isDark ? '#ffffff0d' : '#0000000a',
            tooltipBg: isDark ? '#1e1e2d' : '#ffffff',
            tooltipBorder: isDark ? '#2a2a3d' : '#e5e7eb',
        };
    }

    setupCanvasDPI(canvas: HTMLCanvasElement) {
        const dpr = window.devicePixelRatio || 1;
        const rect = canvas.getBoundingClientRect();

        canvas.width = rect.width * dpr;
        canvas.height = rect.height * dpr;

        const context = canvas.getContext('2d');
        if (context) {
            context.scale(dpr, dpr);
        }
    }

    private createGradient(ctx: CanvasRenderingContext2D, color1: string, color2: string) {
        const gradient = ctx.createLinearGradient(0, 0, 0, ctx.canvas.height * 0.7);
        gradient.addColorStop(0, color1);
        gradient.addColorStop(1, color2);
        return gradient;
    }

    createChart() {
        if (!this.chartInfo || this.chartInfo.length === 0) {
            console.warn('No chart data available');
            return;
        }

        const labels = this.chartInfo.map(data => data.name);
        const dataValue1 = this.chartInfo.map(data => data.value1);
        const dataValue2 = this.chartInfo.map(data => data.value2);

        if (this.chart) {
            this.chart.destroy();
        }

        if (!this.canvasRef) return;
        const canvas = this.canvasRef.nativeElement;
        this.setupCanvasDPI(canvas);

        const ctx = canvas.getContext('2d');
        if (!ctx) return;

        const t = this.chartTheme();

        this.chart = new Chart(canvas, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [
                    {
                        label: this.translate.instant('chart.all_time_records'),
                        data: dataValue1,
                        backgroundColor: this.createGradient(ctx, '#6366f1', '#a5b4fc'),
                        borderRadius: 6,
                        borderSkipped: false,
                        barPercentage: 0.6,
                    },
                    {
                        label: this.translate.instant('chart.used_time_records'),
                        data: dataValue2,
                        backgroundColor: this.createGradient(ctx, '#22c55e', '#86efac'),
                        borderRadius: 6,
                        borderSkipped: false,
                        barPercentage: 0.6,
                    },
                ],
            },
            options: {
                responsive: true,
                maintainAspectRatio: true,
                aspectRatio: 1.8,
                animation: {
                    duration: 800,
                    easing: 'easeOutQuart',
                },
                plugins: {
                    legend: {
                        display: true,
                        position: 'top',
                        labels: {
                            color: t.text,
                            boxWidth: 12,
                            boxHeight: 12,
                            usePointStyle: true,
                            pointStyle: 'rectRounded',
                            padding: 16,
                        },
                    },
                    tooltip: {
                        backgroundColor: t.tooltipBg,
                        titleColor: t.text,
                        bodyColor: t.muted,
                        borderColor: t.tooltipBorder,
                        borderWidth: 1,
                        cornerRadius: 8,
                        padding: 10,
                        boxPadding: 4,
                    },
                },
                scales: {
                    x: {
                        ticks: {
                            color: t.muted,
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
                            color: t.muted,
                            font: {
                                size: 11,
                            },
                            precision: 0,
                        },
                        grid: {
                            color: t.grid,
                        },
                    },
                },
            },
        });
    }

    updateChartLabels() {
        if (this.chart) {
            const allTimeRecordsLabel = this.translate.instant('chart.all_time_records');
            const usedTimeRecordsLabel = this.translate.instant('chart.used_time_records');

            this.chart.data.datasets[0].label = allTimeRecordsLabel;
            this.chart.data.datasets[1].label = usedTimeRecordsLabel;

            this.chart.update('none');
        } else {
            console.warn('Chart is not initialized yet');
        }
    }

    private updateChartTheme() {
        if (this.chart) {
            const t = this.chartTheme();
            this.chart.options.scales.x.ticks.color = t.muted;
            this.chart.options.scales.x.grid.color = t.grid;
            this.chart.options.scales.y.ticks.color = t.muted;
            this.chart.options.scales.y.grid.color = t.grid;
            this.chart.options.plugins.legend.labels.color = t.text;
            this.chart.options.plugins.tooltip.backgroundColor = t.tooltipBg;
            (this.chart as any).options.plugins.tooltip.borderColor = t.tooltipBorder;
            this.chart.update('none');
        }
    }
}
