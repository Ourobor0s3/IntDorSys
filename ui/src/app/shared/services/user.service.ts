import { ApiService } from './api.service';
import { Injectable } from "@angular/core";
import { UserInfoModel } from "../model/userInfo.model";


const apiContactUrl = 'user/';

@Injectable({
    providedIn: 'root',
})
export class UserService {
    public user: UserInfoModel;
    public email: string = '';

    constructor(private api: ApiService) {
    }

    public init(): Promise<UserInfoModel> {
        let promise: Promise<UserInfoModel>;
        if (this.get() == undefined || this.get() == null) {
            promise = this.refreshUser();
        } else {
            promise = new Promise<UserInfoModel>((resolve) => {
                resolve(this.get());
            });
        }
        return promise;
    }

    public clear() {
        this.user = undefined;
    }

    public get(): UserInfoModel {
        return this.user;
    }

    public refreshUser = async (): Promise<UserInfoModel> => {
        return (await this.api
            .get<UserInfoModel>(apiContactUrl))
            .toPromise()
            .then((resp) => {
                this.user = resp.data;
                if (this.isAuthentificated()) return this.user;
                else return new UserInfoModel();
            });
    };

    public isAuthentificated(): boolean {
        let at = localStorage.getItem('accessToken');
        return at != null && at.length > 0;
    }
}
