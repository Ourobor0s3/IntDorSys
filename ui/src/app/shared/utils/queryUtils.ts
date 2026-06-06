export class QueryUtils {
    static objectToQueryString(params: object): string {
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
                        .map(val => `${encodeURIComponent(key)}=${encodeURIComponent(String(val))}`)
                        .join('&');
                }

                if (typeof value === 'object' && value !== null) {
                    value = JSON.stringify(value);
                }

                return `${encodeURIComponent(key)}=${encodeURIComponent(String(value))}`;
            },
        );
        const queryString = encodedEntries.join('&');

        return `?${queryString}`;
    }
}
