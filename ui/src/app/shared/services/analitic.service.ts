import { lastValueFrom } from 'rxjs';
import { ApiService } from './api.service';
import { IResponse } from '../interface/response';
import { Injectable } from "@angular/core";
import { ChartData } from "../interface/chartData.model";


const apiContactUrl = 'analitic';

@Injectable({
    providedIn: 'root',
})
export class AnaliticService {
    constructor(private api: ApiService) {
    }

    async getAnaliticLaund(): Promise<IResponse<ChartData[]>> {
        return await lastValueFrom(await this.api.get<ChartData[]>(apiContactUrl + '/laund'));
    }
}
