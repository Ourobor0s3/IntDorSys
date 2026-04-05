export class BaseFilterModel {
    userId: number;
    skip: number;
    take: number;
    search?: string;
    startDate?: string;
    endDate?: string;
}
