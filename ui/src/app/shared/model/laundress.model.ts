import { UserInfoModel } from "./userInfo.model";

export class LaundressModel {
    selectUser?: UserInfoModel;
    timeWash!: string;
    dateStr?: string;
    timeStr?: string;
}

export class PageLaundressModel {
    date: string;
    laundModels: LaundressModel[];
}
