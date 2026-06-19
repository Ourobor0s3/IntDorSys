import { UserInfoModel } from "./userInfo.model";

export interface LaundressModel {
    selectUser?: UserInfoModel;
    timeWash: string;
    dateStr?: string;
    timeStr?: string;
}

export interface PageLaundressModel {
    date: string;
    laundModels: LaundressModel[];
}
