# 个人记账系统

一个基于 Vue 3 和 .NET 8 的个人记账管理系统，支持移动端使用。

## 功能特性

- 📱 移动端优先设计
- 👤 用户注册与登录
- 💰 收支记录管理
- 📊 数据统计与分析
- 🔐 安全认证机制
- 💾 SQLite 本地数据库

## 技术栈

### 后端
- .NET 8
- Entity Framework Core
- JWT 认证
- SQLite
- Swagger/OpenAPI

### 前端
- Vue 3
- Vant UI
- Vue Router
- Pinia
- Axios

## 项目结构

```
claudecode/
├── backend/                 # .NET 8 后端
│   ├── Controllers/        # API 控制器
│   ├── Models/            # 数据模型
│   ├── Services/          # 业务服务
│   ├── Data/              # 数据库上下文
│   ├── Program.cs         # 程序入口
│   └── appsettings.json   # 配置文件
├── frontend/               # Vue 3 前端
│   ├── src/
│   │   ├── views/         # 页面组件
│   │   ├── stores/        # 状态管理
│   │   ├── services/      # API 服务
│   │   ├── router/        # 路由配置
│   │   └── main.js        # 应用入口
│   └── package.json       # 依赖配置
├── SECURITY.md            # 安全文档
├── API_DOCUMENTATION.md   # API 文档
└── README.md             # 项目说明
```

## 快速开始

### 后端启动

1. 安装 .NET 8 SDK
2. 进入 backend 目录
3. 运行以下命令：

```bash
cd backend
dotnet restore
dotnet run
```

后端服务将在 `http://localhost:5000` 启动，Swagger API 文档在 `http://localhost:5000/swagger`

### 前端启动

1. 安装 Node.js 和 npm
2. 进入 frontend 目录
3. 运行以下命令：

```bash
cd frontend
npm install
npm run serve
```

前端应用将在 `http://localhost:8080` 启动

## 数据库

系统使用 SQLite 数据库，数据库文件将自动创建在 backend 目录下：
- `personal_finance.db` - 主数据库文件

## 配置说明

### 后端配置 (appsettings.json)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=personal_finance.db"
  },
  "JwtSettings": {
    "SecretKey": "您的JWT密钥",
    "Issuer": "PersonalFinanceAPI",
    "Audience": "PersonalFinanceClient"
  }
}
```

### 安全注意事项

1. 生产环境请修改 JWT SecretKey
2. 启用 HTTPS
3. 配置适当的 CORS 策略
4. 定期备份数据库

## API 接口

详细 API 文档请参考 [API_DOCUMENTATION.md](./API_DOCUMENTATION.md)

## 安全特性

详细安全说明请参考 [SECURITY.md](./SECURITY.md)

## 开发说明

### 添加新的交易分类

在后端的 `TransactionService` 中修改 `categoryOptions` 数组

### 自定义统计逻辑

在 `TransactionService.GetStatistics()` 方法中修改统计逻辑

### 前端样式定制

修改 `frontend/src/App.vue` 中的全局样式

## 许可证

MIT License

## 支持

如有问题请提交 Issue 或联系开发团队。