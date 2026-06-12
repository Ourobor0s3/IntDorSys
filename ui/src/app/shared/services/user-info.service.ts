import { ApiService } from './api.service';
import { IResponse } from '../interface/response';
import { Injectable } from "@angular/core";
import { UserInfoModel } from "../model/userInfo.model";
import { UserStatus } from "../enums/UserStatus";


const apiContactUrl = 'users-info';

@Injectable({
    providedIn: 'root',
})
export class UsersInfoService {
    constructor(private api: ApiService) {
    }

    async getUsers(): Promise<IResponse<UserInfoModel[]>> {
        return (await this.api.get<UserInfoModel[]>(apiContactUrl)).toPromise() as Promise<IResponse<UserInfoModel[]>>;

    }

    async getUserById(id: number): Promise<IResponse<UserInfoModel>> {
        return (await this.api.get<UserInfoModel>(`${apiContactUrl}/${id}`)).toPromise() as Promise<IResponse<UserInfoModel>>;
    }

    async changeUserStatus(userId: number, isBlocked: boolean): Promise<IResponse<void>> {
        let newStatus = isBlocked ? UserStatus.Blocked : UserStatus.Registered;
        return (await this.api.put<void>(apiContactUrl + "/change-status/" + userId, newStatus)).toPromise() as Promise<IResponse<void>>;
    }
}
