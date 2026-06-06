export interface IResponse<T> {
    data?: (T);
    errors: Error[];
    isSuccess: boolean;
}

export class Error {
    code?: number;
    message!: string;
}
