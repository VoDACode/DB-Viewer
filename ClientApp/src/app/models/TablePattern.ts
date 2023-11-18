import { TableApiService } from "../services/base-table-api.service";

export type ColumnType = {
    name: string, type: string | ColumnType
}

export class TablePattern {
    public name: string;
    public columns: ColumnType[];
    public api: TableApiService;
    public type: any;

    constructor(name: string, api: TableApiService, type: any) {
        this.name = name;
        this.api = api;
        this.type = type;
        this.columns = TablePattern.columnsFromType(type);
    }

    static columnsFromType(type: any): { name: string, type: any }[] {
        // include subtypes
        const f = (_type: any) => {
            // if _type is a constructor, create an instance of it
            if (typeof _type === 'function')
                _type = new _type();
            const columns: { name: string, type: any }[] = [];
            for (const key in _type) {
                if (_type.hasOwnProperty(key)) {
                    const element = _type[key];
                    if (typeof element === 'object' || typeof element === 'function') {
                        let isConstructor = typeof element === 'function';
                        columns.push({
                            name: key, 
                            type: f(isConstructor ? new element() : element)
                        });
                    }
                    else {
                        columns.push({ name: key, type: typeof element });
                    }
                }
            }
            return columns;
        };
        return f(type);
    }
}