export interface LoginResult {
    success: boolean;
    message: string;
    token?: string;
    userName?: string;
    roles: string[];
}