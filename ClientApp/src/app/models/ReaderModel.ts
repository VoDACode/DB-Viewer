import { LibraryModel } from "./LibraryModel";

export class ReaderModel {
    public id: number = 0;
    public name: string = '';
    public phone: string = '';
    public email: string = '';
    public address: string = '';
    public libraryId: number = 0;
    public library: LibraryModel = new LibraryModel();
}