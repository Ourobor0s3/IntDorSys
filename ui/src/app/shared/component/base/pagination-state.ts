export class PaginationState {
    page = 1;
    pageSize = 15;
    totalPages = 1;

    onFilterChange(): void {
        this.page = 1;
    }

    prev(): boolean {
        if (this.page > 1) {
            this.page--;
            return true;
        }
        return false;
    }

    next(): boolean {
        if (this.page < this.totalPages) {
            this.page++;
            return true;
        }
        return false;
    }

    slice<T>(data: T[]): T[] {
        this.totalPages = Math.max(1, Math.ceil(data.length / this.pageSize));
        const start = (this.page - 1) * this.pageSize;
        return data.slice(start, start + this.pageSize);
    }
}
