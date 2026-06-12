export interface AuditLogModel {
    id: number;
    userId: number;
    userName: string;
    action: string;
    entityName: string;
    entityId: string;
    details: string;
    createdAt: string;
}
