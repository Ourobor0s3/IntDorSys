export interface ChartDataModel<TKey, TValue> {
    name: TKey;
    value1: TValue;
    value2: TValue;
}

export interface ChartData {
    name: string;
    value1: number;
    value2: number;
}
