import { Component, Input, OnDestroy, OnInit, SimpleChanges } from '@angular/core';
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
    @Input() public chartInfo: ChartData[];

    public chart: any;
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
            grid: isDark ? '#2a2a3d' : '#e5e7eb',
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

        const canvas = document.getElementById('custom-chart') as HTMLCanvasElement;
        this.setupCanvasDPI(canvas);

        const t = this.chartTheme();

        new Chart(canvas, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [
                    {
                        label: this.translate.instant('chart.all_time_records'),
                        data: dataValue1,
                        backgroundColor: '#6366f1',
                        borderColor: '#6366f1',
                        borderWidth: 1,
                    },
                    {
                        label: this.translate.instant('chart.used_time_records'),
                        data: dataValue2,
                        backgroundColor: '#22c55e',
                        borderColor: '#22c55e',
                        borderWidth: 1,
                    },
                ],
            },
            options: {
                responsive: true,
                aspectRatio: 2.5,
                plugins: {
                    legend: {
                        display: true,
                        position: 'top',
                        labels: {
                            color: t.text,
                        },
                    },
                },
                scales: {
                    x: {
                        ticks: {
                            color: t.muted,
                        },
                        grid: {
                            color: t.grid,
                        },
                        title: {
                            display: true,
                            text: this.translate.instant('chart.time'),
                            color: t.text,
                        },
                    },
                    y: {
                        beginAtZero: true,
                        ticks: {
                            color: t.muted,
                        },
                        grid: {
                            color: t.grid,
                        },
                        title: {
                            display: true,
                            text: this.translate.instant('chart.count'),
                            color: t.text,
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
            const timeAxisLabel = this.translate.instant('chart.time');
            const countAxisLabel = this.translate.instant('chart.count');

            this.chart.data.datasets[0].label = allTimeRecordsLabel;
            this.chart.data.datasets[1].label = usedTimeRecordsLabel;

            this.chart.options.scales.x.title.text = timeAxisLabel;
            this.chart.options.scales.y.title.text = countAxisLabel;

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
            this.chart.options.scales.x.title.color = t.text;
            this.chart.options.scales.y.ticks.color = t.muted;
            this.chart.options.scales.y.grid.color = t.grid;
            this.chart.options.scales.y.title.color = t.text;
            this.chart.options.plugins.legend.labels.color = t.text;
            this.chart.update('none');
        }
    }
}
