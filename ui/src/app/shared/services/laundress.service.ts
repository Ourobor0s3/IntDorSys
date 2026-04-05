import { ApiService } from './api.service';
import { IResponse } from '../interface/responce';
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
}
