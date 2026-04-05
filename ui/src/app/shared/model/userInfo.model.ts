import { UserStatus } from "../enums/UserStatus";

export class UserInfoModel {
    id!: number;
    fullName?: string;
    numGroup?: string;
    numRoom?: string;
    username!: string;
    isConfirm!: boolean;
    status: UserStatus;
    registerDate: string;
    // показывает, заблочен ли пользователь
    isBlocked: boolean;
}
