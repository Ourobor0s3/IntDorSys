import { Injectable, Renderer2, RendererFactory2 } from '@angular/core';
import { Subject } from 'rxjs';

export type Theme = 'light' | 'dark';

@Injectable({
    providedIn: 'root',
})
export class ThemeService {
    private readonly STORAGE_KEY = 'app-theme';
    private themeSubject = new Subject<Theme>();
    theme$ = this.themeSubject.asObservable();
    private currentTheme: Theme;
    private renderer: Renderer2;

    constructor(rendererFactory: RendererFactory2) {
        this.renderer = rendererFactory.createRenderer(null, null);
        this.currentTheme = (localStorage.getItem(this.STORAGE_KEY) as Theme) || 'light';
        this.applyTheme(this.currentTheme);
    }

    getTheme(): Theme {
        return this.currentTheme;
    }

    toggle(): void {
        const next: Theme = this.currentTheme === 'light' ? 'dark' : 'light';
        this.applyTheme(next);
        this.themeSubject.next(next);
    }

    setTheme(theme: Theme): void {
        this.applyTheme(theme);
        this.themeSubject.next(theme);
    }

    private applyTheme(theme: Theme): void {
        this.currentTheme = theme;
        localStorage.setItem(this.STORAGE_KEY, theme);
        this.renderer.setAttribute(document.documentElement, 'data-bs-theme', theme);
    }
}
