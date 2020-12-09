import { SmartofficePage } from './app.po';

describe('smartoffice App', () => {
  let page: SmartofficePage;

  beforeEach(() => {
    page = new SmartofficePage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
