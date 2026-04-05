import { Component, Input, OnDestroy, OnInit, SimpleChanges } from '@angular/core';
import { Chart, registerables } from "chart.js";
import { ChartData } from "../../model/chartData.model";
import { TranslateService } from '@ngx-translate/core';
import { EventService } from '../../services/event.service';
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
    private destroy$ = new Subject<void>();

    constructor(
        private translate: TranslateService,
        private eventService: EventService,
    ) {
    }

    ngOnInit(): void {
        Chart.register(...registerables);
        this.createChart();

        // Subscribe to language changes
        this.eventService.LangChangeEvent
            .pipe(takeUntil(this.destroy$))
            .subscribe((lang) => {
                this.updateChartLabels();
            });

        // Also subscribe to the TranslateService's onLangChange event
        this.translate.onLangChange
            .pipe(takeUntil(this.destroy$))
            .subscribe(() => {
                this.updateChartLabels();
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

        // Destroy existing chart if it exists
        if (this.chart) {
            this.chart.destroy();
        }

        const canvas = document.getElementById('custom-chart') as HTMLCanvasElement;
        this.setupCanvasDPI(canvas);

        new Chart(canvas, {
            type: 'bar', // Тип диаграммы
            data: {
                labels: labels, // Метки оси X
                datasets: [
                    {
                        label: this.translate.instant('chart.all_time_records'), // Первая метрика
                        data: dataValue1,
                        backgroundColor: 'blue',
                        borderColor: 'blue',
                        borderWidth: 1,
                    },
                    {
                        label: this.translate.instant('chart.used_time_records'), // Вторая метрика
                        data: dataValue2,
                        backgroundColor: 'limegreen',
                        borderColor: 'limegreen',
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
                    },
                },
                scales: {
                    x: {
                        title: {
                            display: true,
                            text: this.translate.instant('chart.time'),
                        },
                    },
                    y: {
                        beginAtZero: true,
                        title: {
                            display: true,
                            text: this.translate.instant('chart.count'),
                        },
                    },
                },
            },
        });
    }

    updateChartLabels() {
        if (this.chart) {
            // Получаем актуальные переводы
            const allTimeRecordsLabel = this.translate.instant('chart.all_time_records');
            const usedTimeRecordsLabel = this.translate.instant('chart.used_time_records');
            const timeAxisLabel = this.translate.instant('chart.time');
            const countAxisLabel = this.translate.instant('chart.count');

            console.log('Translated labels:', {
                allTimeRecordsLabel,
                usedTimeRecordsLabel,
                timeAxisLabel,
                countAxisLabel,
            });

            // Обновляем метки наборов данных
            this.chart.data.datasets[0].label = allTimeRecordsLabel;
            this.chart.data.datasets[1].label = usedTimeRecordsLabel;

            // Обновляем заголовки осей
            this.chart.options.scales.x.title.text = timeAxisLabel;
            this.chart.options.scales.y.title.text = countAxisLabel;

            // Обновляем чарт с принудительным перерисовыванием
            this.chart.update('none'); // 'none' означает без анимации для мгновенного обновления
        } else {
            console.warn('Chart is not initialized yet');
        }
    }
}
