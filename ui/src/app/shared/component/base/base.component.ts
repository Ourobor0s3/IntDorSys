import { LoaderComponent } from 'src/app/shared/component/loader/loader.component';
import { UntypedFormGroup } from "@angular/forms";
import { NgbModal, NgbModalOptions, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmModal } from "../modals/confirm";
import { ModalInfoModel } from "../../model/modalInfo.model";
import { ResultModal } from "../modals/result";
import { BsDatepickerConfig } from "ngx-bootstrap/datepicker";
import { formatDate } from '@angular/common';
import { TranslateService } from '@ngx-translate/core';

export abstract class BaseComponent {

    public passwordPattern = /^[A-Za-z0-9!@#$%^&*()_+-=\[\]{};':"\\|,.<>\/?]+$/;
    public readonly bsConfig: Partial<BsDatepickerConfig> = {
        dateInputFormat: 'DD-MM-YYYY',
        selectWeek: false,
        showWeekNumbers: false,
        isAnimated: true,
        customTodayClass: 'datepicker-custom-today-class',
    };
    private modalRefBase: NgbModalRef | null = null;
    private translationCache: Map<string, string> = new Map();

    protected constructor(
        protected translateBase: TranslateService,
        protected modalServiceBase: NgbModal,
    ) {
    }

    /**
     * Помечает все поля формы как "тронутые" и "грязные"
     * @param formGroup - Форма для обработки
     */
    public markFormGroupTouchedAndDirty(formGroup: UntypedFormGroup): void {
        Object.values(formGroup.controls).forEach((control: UntypedFormGroup) => {
            control.markAsTouched();
            control.markAsDirty();
            control.updateValueAndValidity();
            if (control.controls) {
                this.markFormGroupTouchedAndDirty(control);
            }
        });
    }

    public elemIsInvalid(elem: any): boolean {
        return elem.dirty && !elem.untouched && elem.invalid;
    }

    public textErrorStr(elem: any, namepattern = null) {
        if (this.elemIsInvalid(elem)) {
            let customError = Object.getOwnPropertyNames(elem.errors);
            return elem.errors.required ? this.translateBase.instant("errors.required") :
                elem.errors.max != undefined ? (this.translateBase.instant("errors.max")) :
                    elem.errors.min != undefined ? (this.translateBase.instant("errors.min") + ' ' + elem.errors.min.min) :
                        elem.errors.maxlength != undefined ? (this.translateBase.instant("errors.maxLength") + elem.errors.maxlength.requiredLength) :
                            elem.errors.minlength != undefined ? (this.translateBase.instant("errors.minLength") + elem.errors.minlength.requiredLength) :
                                elem.errors.pattern != undefined && namepattern != null && namepattern == this.passwordPattern ? this.translateBase.instant("errors.passwordPattern") :
                                    elem.errors.email != undefined ? this.translateBase.instant("errors.invalidEmail") :
                                        elem.errors.mismatch != undefined ? this.translateBase.instant("errors.mismatch") :
                                            !!customError && customError.length > 0 ? this.translateBase.instant(customError[0]) : "";
        }
        return "";
    }

    /**
     * Управляет отображением индикатора загрузки
     * @param isLoading - Флаг активности загрузки
     */
    public setLoading(isLoading: boolean): void {
        LoaderComponent.setLoading(isLoading);
    }

    /**
     * Возвращает настройки для модального окна
     * @param size - Размер окна
     */
    public getModalOptions(size: string = 'md'): NgbModalOptions {
        return {
            backdropClass: 'light-white-backdrop',
            centered: true,
            size: size,
            windowClass: 'super-modal-delete-users very-nice-shadow',
        };
    }

    /**
     * Форматирует дату в строку
     * @param value - Дата для форматирования
     * @param format - Формат даты
     * @param locale - Локаль
     * @param timezone - Часовой пояс
     */
    public formatDate(value: string | number | Date, format: string, locale: string, timezone?: string): string {
        return formatDate(value, format, locale, timezone);
    }

    /**
     * Устанавливает ширину календаря равной ширине входного поля
     * @param calendarId - ID элемента календаря
     */
    public setCalendarWidth(calendarId = '#bsCalendar'): void {
        const input = document.querySelector<HTMLElement>(calendarId);
        if (!input) return;

        const width = input.offsetWidth;
        document.querySelectorAll<HTMLElement>('.bs-calendar-container').forEach(calendar => {
            calendar.style.width = `${width}px`;
        });
    }

    /**
     * Обрабатывает клик по календарю (фокус/разфокус)
     * @param calendarId - ID элемента календаря
     */
    public calendarClick(calendarId = '#bsCalendar'): void {
        const input = document.querySelector<HTMLInputElement>(calendarId);
        if (!input) return;

        if (document.activeElement !== input) {
            input.focus();
        } else {
            input.blur();
        }
    }

    /**
     * Возвращает диапазон дат с заданным смещением
     * @param daysDelta - Смещение в днях
     * @param monthDelta - Смещение в месяцах
     * @param yearDelta - Смещение в годах
     */
    public GetCurrentDateWithDelta(
        daysDelta = 2,
        monthDelta = 0,
        yearDelta = 0,
    ): { dateStart: Date; dateEnd: Date } {
        const { dateStart, dateEnd } = this.GetCurrentDateWithDeltaStr(daysDelta, monthDelta, yearDelta);
        return {
            dateStart: new Date(dateStart),
            dateEnd: new Date(dateEnd),
        };
    }

    /**
     * Возвращает диапазон дат в строковом формате с заданным смещением
     * @param daysDelta - Смещение в днях
     * @param monthDelta - Смещение в месяцах
     * @param yearDelta - Смещение в годах
     */
    public GetCurrentDateWithDeltaStr(
        daysDelta: number = 2,
        monthDelta: number = 0,
        yearDelta: number = 0,
    ): { dateStart: string; dateEnd: string } {
        const today = new Date();
        const { year, month, day } = {
            year: today.getFullYear(),
            month: today.getMonth(),
            day: today.getDate(),
        };

        const dateStart = new Date(year - yearDelta, month - monthDelta, day - daysDelta / 2).toISOString();
        const dateEnd = new Date(year, month, day + daysDelta * 3).toISOString();

        return { dateStart, dateEnd };
    }

    public GetTodayDate(): Date {
        const today = new Date();
        const { year, month, day } = {
            year: today.getFullYear(),
            month: today.getMonth(),
            day: today.getDate(),
        };
        return new Date(year, month, day);
    }

    // if system error return true
    protected showResponseError(response: any, title: string = null): boolean {

        if (!response || !response.error && !response.errors || response.status === 401) {
            return false;
        }

        if (!!response.errors && response.errors.length != 0) {
            this.showError(response.errors[0].message, title);
            return false;
        }

        if (typeof response.error === 'string') {
            this.showError(response.error, title);
            return false;
        }

        if (!!response.error.errors && response.error.errors.length != 0) {
            for (let e of response.error.errors) {
                if (e.message == 'Chart loading error') continue;

                this.showError(e.message, title);
            }

            return false;
        }

        if (!!response.error.error && !!response.error.error_description) {
            this.showError(response.error.error_description, title);
            return false;
        }

        if (!navigator.onLine) {
            this.showError(
                'Unable to connect to the Internet. Please check your network connection',
                'Failed to Load Resource',
            );
            return false;
        }

        this.showError("System error, please contact support", title);
        return true;
    }

    /**
     * Показывает модальное окно с сообщением об успехе
     * @param message - Текст сообщения
     * @param titleMes - Заголовок окна
     * @param maxWidth - Максимальная ширина окна
     */
    protected showSuccess(message: string = '', titleMes: string = 'Success', maxWidth: string = '360px'): Promise<any> {
        return this.showResultModal(message, titleMes, maxWidth);
    }

    /**
     * Показывает модальное окно с сообщением об ошибке
     * @param message - Текст сообщения
     * @param titleMes - Заголовок окна
     * @param maxWidth - Максимальная ширина окна
     */
    protected showError(message: string = '', titleMes: string = 'Error', maxWidth: string = '360px'): Promise<any> {
        return this.showResultModal(message, titleMes, maxWidth, 'btn-danger');
    }

    /**
     * Показывает модальное окно с подтверждением действия
     * @param title - Заголовок окна
     * @param message - Текст сообщения
     * @param showDeclineButton - Показывать ли кнопку отмены
     * @param confirmButtonText - Текст кнопки подтверждения
     */
    protected showConfirm(
        title: string,
        message: string,
        showDeclineButton: boolean = true,
        confirmButtonText: string = "Ok",
    ): Promise<any> {
        const modalInfo = new ModalInfoModel();
        modalInfo.title = title;
        modalInfo.description = message;
        modalInfo.showDeclineButton = showDeclineButton;
        modalInfo.buttonConfirm = confirmButtonText;
        return this.showModal(modalInfo);
    }

    /**
     * Показывает информационное модальное окно
     * @param htmlMessage - HTML-текст сообщения
     */
    protected showInfo(htmlMessage: string): Promise<any> {
        const modalInfo = new ModalInfoModel();
        modalInfo.description = htmlMessage;
        modalInfo.showDeclineButton = false;
        modalInfo.showConfirmButton = false;
        modalInfo.showErrorButton = false;
        modalInfo.showTitle = false;
        return this.showModal(modalInfo);
    }

    /**
     * Базовый метод для отображения модального окна
     * @param modalInfo - Конфигурация модального окна
     */
    protected showModal(modalInfo: ModalInfoModel): Promise<any> {
        this.modalRefBase = this.modalServiceBase.open(ConfirmModal, this.getModalOptions());
        const component = this.modalRefBase.componentInstance;

        // Set modal content
        component.title = modalInfo.title;
        component.description = modalInfo.description;
        component.showDescription = modalInfo.showDescription;
        component.showTitle = modalInfo.showTitle;

        // Set button configuration
        component.showConfirmButton = modalInfo.showConfirmButton;
        component.showDeclineButton = modalInfo.showDeclineButton;
        component.showErrorButton = modalInfo.showErrorButton;
        component.buttonConfirm = modalInfo.buttonConfirm;
        component.buttonDecline = modalInfo.buttonDecline;
        component.buttonDeclineFontSize = modalInfo.buttonDeclineFontSize;
        component.buttonError = modalInfo.buttonError;

        return this.modalRefBase.result;
    }

    /**
     * Показывает всплывающее уведомление
     * @param message - Текст сообщения
     * @param duration - Длительность отображения в миллисекундах
     */
    protected showToast(message: string, duration: number = 3000): void {
        const cachedMessage = this.getCachedTranslation(message);
        const toast = document.createElement('div');
        toast.textContent = cachedMessage;
        toast.className = 'toast-notification';

        document.body.appendChild(toast);

        requestAnimationFrame(() => {
            toast.classList.add('show');
        });

        setTimeout(() => {
            toast.classList.remove('show');
            toast.addEventListener('transitionend', () => {
                toast.remove();
            });
        }, duration);
    }

    /**
     * Внутренний метод для отображения модального окна с результатом
     */
    private showResultModal(message: string, titleMes: string, maxWidth: string, buttonClass: string = ''): Promise<any> {
        this.modalRefBase = this.modalServiceBase.open(ResultModal, this.getModalOptions());
        const component = this.modalRefBase.componentInstance;

        component.title = titleMes;
        component.description = message;
        component.showDescription = !!message;
        component.maxWidth = maxWidth;
        if (buttonClass) {
            component.buttonClassList = buttonClass;
        }

        return this.modalRefBase.result;
    }

    /**
     * Получает перевод из кэша или выполняет новый перевод
     * @param key - Ключ перевода
     */
    private getCachedTranslation(key: string): string {
        if (!this.translationCache.has(key)) {
            this.translationCache.set(key, this.translateBase.instant(key));
        }
        return this.translationCache.get(key) || key;
    }
}
