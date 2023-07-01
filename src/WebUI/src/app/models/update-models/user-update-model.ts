export interface UserUpdateModel {
    userName?: string;
    email?: string;
    roles?: string[];
    firstName?: string;
    lastName?: string;
    [key: string]: any;
}