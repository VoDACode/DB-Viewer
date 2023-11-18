export type SqlParam = {
    position: number;
    mode: 'IN' | 'OUT' | 'INOUT';
    name: string;
    type: string;
    isResult: boolean;
    value: string;
};