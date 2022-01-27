import { Result } from "./result";

export interface Team {
    start: Date;
    end: Date;
    teamName: string;
    personName: string;
    eggs: number;

    results?: Result[];
}
