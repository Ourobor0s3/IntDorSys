import { Injectable } from '@angular/core';
import { ApiService } from "./api.service";
import { IResponse } from "../interface/responce";

const accountApiUrl = 'account/';

export interface IRegister {
    telegramId: string;
    email: string;
    password: string;
}


@Injectable({
    providedIn: 'root',
})
export class AccountService {
    public email: string = '';

    constructor(
        private api: ApiService,
    ) {
    }

    /**
     * Регистрация нового юзера
     * @param registerModel Данные о регистрации
     */
    register(registerModel: IRegister): Promise<IResponse<void>> {
        this.email = registerModel.email
        return this.api.postAnonym<void>(accountApiUrl, registerModel).toPromise();
    }
}
