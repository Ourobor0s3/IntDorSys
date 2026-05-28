import { ApiService } from './api.service';
import { IResponse } from '../interface/response';
import { PageLaundressModel } from "../model/laundress.model";
import { Injectable } from "@angular/core";
import { BaseFilterModel } from "../model/filter/baseFilter.model";
import { QueryUtils } from "../utils/queryUtils";
import { ReportModel } from "../model/report.model";

const apiLaundUrl = 'laund';

@Injectable({
    providedIn: 'root',
})
export class LaundressService {
    constructor(private api: ApiService) {
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
}
