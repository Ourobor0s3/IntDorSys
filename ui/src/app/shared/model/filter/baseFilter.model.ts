export class BaseFilterModel {
    userId: number = 0;
    skip: number = 0;
    take: number = 10;
    search?: string;
    startDate?: string;
    endDate?: string;
}
