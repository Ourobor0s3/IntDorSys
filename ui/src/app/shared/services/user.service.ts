import { ApiService } from './api.service';
import { Injectable } from "@angular/core";
import { UserInfoModel } from "../interface/userInfo.model";


const apiContactUrl = 'user/';

@Injectable({
    providedIn: 'root',
})
export class UserService {
    public user: UserInfoModel | undefined;
    public email: string = '';

    constructor(private api: ApiService) {
    }

    public init(): Promise<UserInfoModel | undefined> {
        if (this.user == null) {
            return this.refreshUser();
        }
        return Promise.resolve(this.user);
    }

    public clear() {
        this.user = undefined;
    }

    public get(): UserInfoModel | undefined {
        return this.user;
    }

    public refreshUser = async (): Promise<UserInfoModel | undefined> => {
        const resp = await this.api.get<UserInfoModel>(apiContactUrl);
        this.user = resp.data;
        return this.user;
    };

    public isAuthenticated(): boolean {
        let raw = localStorage.getItem('auth');
        if (!raw) return false;
        try {
            let data = JSON.parse(raw);
            return !!data.accessToken;
        } catch {
            return false;
        }
    }
}
