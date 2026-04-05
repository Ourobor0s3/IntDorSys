import { FileInfoModel } from "./fileInfo.model";

export class ReportModel {
    userId: number;
    createdAt: Date;
    username: string;
    groupId: string;
    description: string;
    files: FileInfoModel[];
}
