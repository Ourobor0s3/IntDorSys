export interface IResponse<T> {
    data?: (T);
    errors: ApiError[];
    isSuccess: boolean;
}

export interface ApiError {
    code?: number;
    message: string;
}
