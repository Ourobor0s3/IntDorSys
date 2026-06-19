import { UserStatus } from "../enums/UserStatus";
import { WashRecordModel } from "./washRecord.model";

export interface UserInfoModel {
    id: number;
    fullName?: string;
    numGroup?: string;
    numRoom?: string;
    username: string;
    isConfirm: boolean;
    status: UserStatus;
    registerDate: string;
    isBlocked: boolean;
    usageCount: number;
    recentWashes: WashRecordModel[];
    roles?: string[];
}
