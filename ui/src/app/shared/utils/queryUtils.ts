export class QueryUtils {
    static objectToQueryString(params: Record<string, any>): string {
        if (!params)
            return '';

        const entries = Object.entries(params).filter(
            ([_, value]) => value !== null && value !== undefined && value != -200,
        );

        if (entries.length === 0) {
            return '';
        }

        const encodedEntries = entries.map(
            ([key, value]) => {
                if (Array.isArray(value)) {
                    return value
                        .map(val => `${encodeURIComponent(key)}=${encodeURIComponent(val)}`)
                        .join('&');
                }

                if (value instanceof Object) {
                    value = JSON.stringify(value);
                }

                return `${encodeURIComponent(key)}=${encodeURIComponent(value)}`;
            },
        );
        const queryString = encodedEntries.join('&');

        return `?${queryString}`;
    }
}
