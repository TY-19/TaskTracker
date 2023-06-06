import { Assignment } from "./assignment";
import { Employee } from "./employee";
import { Stage } from "./stage";

export interface Board {
    id: number;
    name: string;
    stages?: Stage[];
    employees?: Employee[];
    assignments?: Assignment[];
}