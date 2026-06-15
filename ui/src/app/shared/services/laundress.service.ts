import { ApiService } from './api.service';
import { HttpClient } from '@angular/common/http';
import { IResponse } from '../interface/response';
import { PageLaundressModel } from "../model/laundress.model";
import { Injectable, Renderer2, RendererFactory2 } from "@angular/core";
import { lastValueFrom } from "rxjs";
import { BaseFilterModel } from "../model/filter/baseFilter.model";
import { QueryUtils } from "../utils/queryUtils";
import { ReportModel } from "../model/report.model";
import { AuditLogModel } from "../interface/audit-log";
import { environment } from "../../../environments/environment";

const apiLaundUrl = 'laund';

@Injectable({
    providedIn: 'root',
})
export class LaundressService {
    private renderer: Renderer2;

    constructor(private api: ApiService, private http: HttpClient, rendererFactory: RendererFactory2) {
        this.renderer = rendererFactory.createRenderer(null, null);
    }

    async getLaund(filter: BaseFilterModel): Promise<IResponse<PageLaundressModel[]>> {
        let url = apiLaundUrl + QueryUtils.objectToQueryString(filter)
        return await lastValueFrom(await this.api.get<PageLaundressModel[]>(url));
    }

    async getReports(filter: BaseFilterModel): Promise<IResponse<ReportModel[]>> {
        let url = apiLaundUrl + "/reports" + QueryUtils.objectToQueryString(filter);
        return await lastValueFrom(await this.api.get<ReportModel[]>(url));
    }

    async createTime(timeWash: string, createdUserId: number): Promise<IResponse<boolean>> {
        return await lastValueFrom(await this.api.post<boolean>(apiLaundUrl, { timeWash, createdUserId }));
    }

    async bookUser(timeWash: string, userId: number): Promise<IResponse<boolean>> {
        return await lastValueFrom(await this.api.post<boolean>(apiLaundUrl + '/book', { timeWash, userId }));
    }

    async unbookUser(timeWash: string, userId: number): Promise<IResponse<boolean>> {
        let url = apiLaundUrl + '/book?timeWash=' + encodeURIComponent(timeWash) + '&userId=' + userId;
        return await lastValueFrom(await this.api.delete<boolean>(url));
    }

    async createTimeRange(date: string, startHour: number, endHour: number): Promise<IResponse<number>> {
        return await lastValueFrom(await this.api.post<number>(apiLaundUrl + '/range', { date, startHour, endHour }));
    }

    async deleteTime(timeWash: string): Promise<IResponse<boolean>> {
        let url = apiLaundUrl + '?timeWash=' + encodeURIComponent(timeWash);
        return await lastValueFrom(await this.api.delete<boolean>(url));
    }

    async getAudit(filter: BaseFilterModel): Promise<IResponse<AuditLogModel[]>> {
        let url = apiLaundUrl + '/audit' + QueryUtils.objectToQueryString(filter);
        return await lastValueFrom(await this.api.get<AuditLogModel[]>(url));
    }

    async exportExcel(startDate: string, endDate: string): Promise<void> {
        let url = environment.apiUrl + apiLaundUrl + '/export?startDate=' + encodeURIComponent(startDate) + '&endDate=' + encodeURIComponent(endDate);
        let blob = await lastValueFrom(this.http.get(url, { responseType: 'blob' }));
        let link = this.renderer.createElement('a');
        this.renderer.setAttribute(link, 'href', window.URL.createObjectURL(blob));
        this.renderer.setAttribute(link, 'download', 'laundress_export_' + new Date().toISOString().split('T')[0] + '.xlsx');
        link.click();
        window.URL.revokeObjectURL(link.href);
    }
}
