import { Component } from '@angular/core';
import { SqlParam } from 'src/app/models/SqlParam';
import { SqlApiService } from 'src/app/services/sql-api.service';

type SqlResult = {
  table: string;
  columns: string[];
  rows: {
    [key: string]: string;
  }[];
};

@Component({
  selector: 'app-sql',
  templateUrl: './sql.component.html',
  styleUrls: ['./sql.component.css']
})
export class SqlComponent {
  targetType: 'function' | 'procedure' = 'procedure';

  targetObjects: { name: string; selected: boolean }[] = [];

  targetParams: SqlParam[] = [];

  result: SqlResult[] = [];

  get onlyInParams(): SqlParam[] {
    return this.targetParams.filter((param) => param.mode === 'IN');
  }

  constructor(private sqlApiService: SqlApiService) {
    this.loadNames();
  }

  selectType(type: 'function' | 'procedure'): void {
    this.targetType = type;
    this.loadNames();
  }

  private loadNames(): void {
    if (this.targetType === 'function') {
      this.sqlApiService.getFunctionNames().subscribe((names) => {
        this.targetObjects = names.map((name) => {
          return {
            name: name,
            selected: false
          };
        });
        this.targetObjects[0].selected = true;
        this.selectName(this.targetObjects[0].name);
      });
    } else {
      this.sqlApiService.getProcedureNames().subscribe((names) => {
        this.targetObjects = names.map((name) => {
          return {
            name: name,
            selected: false
          };
        });
        this.targetObjects[0].selected = true;
        this.selectName(this.targetObjects[0].name);
      });
    }
  }

  selectName(name: string): void {
    this.targetObjects.forEach((obj) => {
      obj.selected = obj.name === name;
    });
    this.sqlApiService.getParams(name).subscribe((params) => {
      this.targetParams = params;
    });
  }

  run(): void {
    var selectedName = this.targetObjects.find((name) => name.selected)?.name;
    if (!selectedName) {
      return;
    }
    if (this.targetType === 'function') {
      this.sqlApiService
        .runFunction(selectedName, this.onlyInParams)
        .subscribe((result) => {
          this.parseResult(result);
        });
    } else if (this.targetType === 'procedure') {
      this.sqlApiService
        .runProcedure(selectedName, this.onlyInParams)
        .subscribe((result) => {
          this.parseResult(result);
        });
    }
  }

  parseResult(result: any): void {
    const parser = new DOMParser()
    const xmlDoc = parser.parseFromString(result, "text/xml");
    const doc = xmlDoc.getElementsByTagName('DocumentElement')[0];
  
    let tableNames = Array.from(doc.children).map((table) => table.tagName);
    tableNames = Array.from(new Set(tableNames));
    const columns = Array.from(doc.children[0].children).map((column) => column.tagName);
    const rows = Array.from(doc.children).map((table) => {
      const row: any = {};
      Array.from(table.children).forEach((column) => {
        row[column.tagName] = column.textContent;
      });
      return row;
    });
    this.result = tableNames.map((tableName, index) => {
      return {
        table: tableName,
        columns: columns,
        rows: rows
      }
    });
  }

}
