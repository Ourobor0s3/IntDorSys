import { FileInfoModel } from "./fileInfo.model";

export interface ReportModel {
    userId: number;
    createdAt: Date;
    username: string;
    groupId: string;
    description: string;
    files: FileInfoModel[];
}
