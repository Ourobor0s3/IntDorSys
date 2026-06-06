import { ApiService } from './api.service';
import { HttpClient } from '@angular/common/http';
import { IResponse } from '../interface/response';
import { PageLaundressModel } from "../model/laundress.model";
import { Injectable } from "@angular/core";
import { lastValueFrom } from "rxjs";
import { BaseFilterModel } from "../model/filter/baseFilter.model";
import { QueryUtils } from "../utils/queryUtils";
import { ReportModel } from "../model/report.model";
import { environment } from "../../../environments/environment";

const apiLaundUrl = 'laund';

export interface AuditLogModel {
    id: number;
    userId: number;
    userName: string;
    action: string;
    entityName: string;
    entityId: string;
    details: string;
    createdAt: string;
}

@Injectable({
    providedIn: 'root',
})
export class LaundressService {
    constructor(private api: ApiService, private http: HttpClient) {
    }

    async getLaund(filter: BaseFilterModel): Promise<IResponse<PageLaundressModel[]>> {
        let url = apiLaundUrl + QueryUtils.objectToQueryString(filter)
        return (await this.api.get<PageLaundressModel[]>(url)).toPromise() as Promise<IResponse<PageLaundressModel[]>>;
    }

    async getReports(filter: BaseFilterModel): Promise<IResponse<ReportModel[]>> {
        let url = apiLaundUrl + "/reports" + QueryUtils.objectToQueryString(filter);
        return (await this.api.get<ReportModel[]>(url)).toPromise() as Promise<IResponse<ReportModel[]>>;
    }

    async createTime(timeWash: string, createdUserId: number): Promise<IResponse<boolean>> {
        return (await this.api.post<boolean>(apiLaundUrl, { timeWash, createdUserId })).toPromise() as Promise<IResponse<boolean>>;
    }

    async bookUser(timeWash: string, userId: number): Promise<IResponse<boolean>> {
        return (await this.api.post<boolean>(apiLaundUrl + '/book', { timeWash, userId })).toPromise() as Promise<IResponse<boolean>>;
    }

    async unbookUser(timeWash: string, userId: number): Promise<IResponse<boolean>> {
        let url = apiLaundUrl + '/book?timeWash=' + encodeURIComponent(timeWash) + '&userId=' + userId;
        return (await this.api.delete<boolean>(url)).toPromise() as Promise<IResponse<boolean>>;
    }

    async createTimeRange(date: string, startHour: number, endHour: number, createdUserId: number): Promise<IResponse<number>> {
        return (await this.api.post<number>(apiLaundUrl + '/range', { date, startHour, endHour, createdUserId })).toPromise() as Promise<IResponse<number>>;
    }

    async deleteTime(timeWash: string): Promise<IResponse<boolean>> {
        let url = apiLaundUrl + '?timeWash=' + encodeURIComponent(timeWash);
        return (await this.api.delete<boolean>(url)).toPromise() as Promise<IResponse<boolean>>;
    }

    async getAudit(filter: BaseFilterModel): Promise<IResponse<AuditLogModel[]>> {
        let url = apiLaundUrl + '/audit' + QueryUtils.objectToQueryString(filter);
        return (await this.api.get<AuditLogModel[]>(url)).toPromise() as Promise<IResponse<AuditLogModel[]>>;
    }

    async exportExcel(startDate: string, endDate: string): Promise<void> {
        let url = environment.apiUrl + apiLaundUrl + '/export?startDate=' + encodeURIComponent(startDate) + '&endDate=' + encodeURIComponent(endDate);
        let blob = await lastValueFrom(this.http.get(url, { responseType: 'blob' }));
        let link = document.createElement('a');
        link.href = window.URL.createObjectURL(blob);
        link.download = 'laundress_export_' + new Date().toISOString().split('T')[0] + '.xlsx';
        link.click();
        window.URL.revokeObjectURL(link.href);
    }
}
