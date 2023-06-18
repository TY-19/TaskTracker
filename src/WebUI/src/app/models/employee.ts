export interface Employee {
    id: number;
    userName: string;
    roles: string[];
    firstName?: string;
    lastName?: string;
    [key: string]: any;
}