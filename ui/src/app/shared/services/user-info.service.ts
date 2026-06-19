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

    getUsers(): Promise<IResponse<UserInfoModel[]>> {
        return this.api.get<UserInfoModel[]>(apiContactUrl);
    }

    getUserById(id: number): Promise<IResponse<UserInfoModel>> {
        return this.api.get<UserInfoModel>(`${apiContactUrl}/${id}`);
    }

    changeUserStatus(userId: number, isBlocked: boolean): Promise<IResponse<void>> {
        let newStatus = isBlocked ? UserStatus.Blocked : UserStatus.Registered;
        return this.api.put<void>(apiContactUrl + '/change-status/' + userId, newStatus);
    }

    confirmUser(userId: number, roleKey: string = 'Student'): Promise<IResponse<void>> {
        return this.api.put<void>(apiContactUrl + '/confirm/' + userId + '?roleKey=' + roleKey, {});
    }

    removeRole(userId: number, roleKey: string): Promise<IResponse<void>> {
        return this.api.put<void>(apiContactUrl + '/remove-role/' + userId + '?roleKey=' + roleKey, {});
    }
}
