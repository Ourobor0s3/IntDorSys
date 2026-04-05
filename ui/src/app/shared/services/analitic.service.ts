import { ApiService } from './api.service';
import { IResponse } from '../interface/responce';
import { Injectable } from "@angular/core";
import { ChartData } from "../model/chartData.model";


const apiContactUrl = 'analitic';

@Injectable({
    providedIn: 'root',
})
export class AnaliticService {
    constructor(private api: ApiService) {
    }

    async getAnaliticLaund(): Promise<IResponse<ChartData[]>> {
        return (await this.api.get<ChartData[]>(apiContactUrl + '/laund')).toPromise() as Promise<IResponse<ChartData[]>>;

    }
}
