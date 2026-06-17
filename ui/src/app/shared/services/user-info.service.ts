import { lastValueFrom } from 'rxjs';
import { ApiService } from './api.service';
import { IResponse } from '../interface/response';
import { Injectable } from "@angular/core";
import { UserInfoModel } from "../interface/userInfo.model";
import { UserStatus } from "../enums/UserStatus";


const apiContactUrl = 'users-info';

@Injectable({
    providedIn: 'root',
})
export class UsersInfoService {
    constructor(private api: ApiService) {
    }

    async getUsers(): Promise<IResponse<UserInfoModel[]>> {
        return await lastValueFrom(await this.api.get<UserInfoModel[]>(apiContactUrl));
    }

    async getUserById(id: number): Promise<IResponse<UserInfoModel>> {
        return await lastValueFrom(await this.api.get<UserInfoModel>(`${apiContactUrl}/${id}`));
    }

    async changeUserStatus(userId: number, isBlocked: boolean): Promise<IResponse<void>> {
        let newStatus = isBlocked ? UserStatus.Blocked : UserStatus.Registered;
        return await lastValueFrom(await this.api.put<void>(apiContactUrl + "/change-status/" + userId, newStatus));
    }

    async confirmUser(userId: number, roleKey: string = 'Student'): Promise<IResponse<void>> {
        return await lastValueFrom(await this.api.put<void>(apiContactUrl + "/confirm/" + userId + "?roleKey=" + roleKey, {}));
    }

    async removeRole(userId: number, roleKey: string): Promise<IResponse<void>> {
        return await lastValueFrom(await this.api.put<void>(apiContactUrl + "/remove-role/" + userId + "?roleKey=" + roleKey, {}));
    }
}
