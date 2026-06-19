import { Component, ElementRef, Input, OnDestroy, OnInit, SimpleChanges, ViewChild } from '@angular/core';
import { Chart, registerables } from 'chart.js';
import { ChartData } from '../../interface/chartData.model';
import { EventService } from '../../services/event.service';
import { ThemeService } from '../../services/theme.service';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { ChartConfigFactory } from './bar-chart.config';

@Component({
    selector: 'app-bar-chart',
    templateUrl: './bar-chart.component.html',
})
export class BarChartComponent implements OnInit, OnDestroy {
    @ViewChild('customChart', { static: false }) canvasRef!: ElementRef<HTMLCanvasElement>;
    @Input() public chartInfo: ChartData[];
    @Input() public datasetLabels: { label1: string; label2: string };

    public chart: Chart | null;
    private currentTheme: 'light' | 'dark' = 'light';
    private destroy$ = new Subject<void>();

    constructor(
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
            .subscribe(() => this.updateChartLabels());

        this.themeService.theme$
            .pipe(takeUntil(this.destroy$))
            .subscribe((theme) => {
                this.currentTheme = theme;
                this.updateChartTheme();
            });
    }

    ngOnChanges(changes: SimpleChanges): void {
        if (changes['chartInfo'] && this.chartInfo?.length) {
            this.createChart();
        }

        if (changes['datasetLabels'] && this.chart) {
            this.updateChartLabels();
        }
    }

    ngOnDestroy(): void {
        this.destroy$.next();
        this.destroy$.complete();
    }

    createChart() {
        if (!this.chartInfo?.length || !this.canvasRef || !this.datasetLabels) {
            return;
        }

        if (this.chart) {
            this.chart.destroy();
        }

        const canvas = this.canvasRef.nativeElement;
        this.setupCanvasDPI(canvas);

        const ctx = canvas.getContext('2d');
        if (!ctx) return;

        this.chart = new Chart(canvas, {
            type: 'bar',
            data: ChartConfigFactory.buildChartData(this.chartInfo, ctx, this.datasetLabels),
            options: ChartConfigFactory.buildChartOptions(ChartConfigFactory.chartTheme(this.currentTheme)),
        });
    }

    updateChartLabels() {
        if (!this.chart || !this.datasetLabels) return;

        this.chart.data.datasets[0].label = this.datasetLabels.label1;
        this.chart.data.datasets[1].label = this.datasetLabels.label2;
        this.chart.update('none');
    }

    private updateChartTheme() {
        if (!this.chart) return;

        const t = ChartConfigFactory.chartTheme(this.currentTheme);
        this.chart.options.scales.x.ticks.color = t.muted;
        this.chart.options.scales.x.grid.color = t.grid;
        this.chart.options.scales.y.ticks.color = t.muted;
        this.chart.options.scales.y.grid.color = t.grid;
        this.chart.options.plugins.legend.labels.color = t.text;
        this.chart.options.plugins.tooltip.backgroundColor = t.tooltipBg;
        this.chart.options.plugins.tooltip.borderColor = t.tooltipBorder;
        this.chart.update('none');
    }

    private setupCanvasDPI(canvas: HTMLCanvasElement) {
        const dpr = window.devicePixelRatio || 1;
        const rect = canvas.getBoundingClientRect();

        canvas.width = rect.width * dpr;
        canvas.height = rect.height * dpr;

        const context = canvas.getContext('2d');
        context?.scale(dpr, dpr);
    }
}
