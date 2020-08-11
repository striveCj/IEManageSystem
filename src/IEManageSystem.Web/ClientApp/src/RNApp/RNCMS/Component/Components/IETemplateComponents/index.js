// 注册各个模板所需的部件
import Container from "./Container"
import IELogo from './IELogo'

export default (register) => {
    Container(register)
    IELogo(register)
};