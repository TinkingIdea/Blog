<template>
  <div>
    <!-- 面包屑导航区域 -->
    <el-breadcrumb separator-class="el-icon-arrow-right">
      <el-breadcrumb-item :to="{ path: '/home' }">首页</el-breadcrumb-item>
      <el-breadcrumb-item>系统管理</el-breadcrumb-item>
      <el-breadcrumb-item>菜单管理</el-breadcrumb-item>
    </el-breadcrumb>

    <el-card>
      <el-row :gutter="20">
        <el-col :span="8">
          <el-input
            placeholder="请输入内容"
            v-model="queryInfo.query"
            class="input-with-select"
            clearable
            @clear="getMenuList"
          >
            <el-button
              slot="append"
              icon="el-icon-search"
              @click="getMenuList"
            ></el-button>
          </el-input>
        </el-col>
        <el-col :span="4">
          <el-button type="primary" @click="showAddDialog">添加菜单</el-button>
        </el-col>
      </el-row>

      <tree-table
        class="treeTable"
        :data="menuList"
        :columns="columns"
        :selection-type="false"
        :expand-type="false"
        show-index
        index-text="#"
        border
        :show-row-hover="false"
      >
        <template slot="state" slot-scope="scope">
          <el-switch
            v-model="scope.row.state"
            @change="changeMenuState(scope.row)"
          ></el-switch>
        </template>

        <template slot="opt" slot-scope="scope">
          <el-button
            type="primary"
            size="mini"
            icon="el-icon-edit"
            @click="showEditDialog(scope.row)"
            >编辑</el-button
          >
        </template>
      </tree-table>

      <el-pagination
        @size-change="handleSizeChange"
        @current-change="handleCurrentChange"
        :current-page="queryInfo.pagenum"
        :page-sizes="[1, 5, 10, 50]"
        :page-size="5"
        layout="total, sizes, prev, pager, next, jumper"
        :total="total"
        background
      >
      </el-pagination>
    </el-card>

    <el-dialog
      title="添加菜单"
      :visible.sync="addDialogVisible"
      width="50%"
      @close="addDialogClosed"
    >
      <el-form
        :model="addForm"
        :rules="addFormRules"
        ref="addFormRef"
        label-width="100px"
      >
        <el-form-item label="菜单名称" prop="name">
          <el-input v-model="addForm.name"></el-input>
        </el-form-item>
        <el-form-item label="路径">
          <el-input v-model="addForm.path"></el-input>
        </el-form-item>
        <el-form-item label="序号">
          <el-input v-model="addForm.order" type="number"></el-input>
        </el-form-item>
        <el-form-item label="图标" prop="icon">
          <el-input v-model="addForm.icon"></el-input>
          <i :class="addForm.icon"></i>
        </el-form-item>
        <el-form-item label="根菜单">
          <el-select v-model="addForm.level" placeholder="请选择">
            <el-option
              v-for="item in rootMenuList"
              :key="item.value"
              :label="item.label"
              :value="item.value + ''"
            >
            </el-option>
          </el-select>
        </el-form-item>
      </el-form>
      <span slot="footer" class="dialog-footer">
        <el-button @click="addDialogVisible = false">取 消</el-button>
        <el-button type="primary" @click="addMenu">确 定</el-button>
      </span>
    </el-dialog>

    <el-dialog
      title="修改菜单"
      :visible.sync="editDialogVisible"
      width="50%"
      @close="editDialogClosed"
    >
      <el-form
        :model="editForm"
        :rules="editFormRules"
        ref="editFormRef"
        label-width="100px"
      >
        <el-form-item label="菜单名称" prop="name">
          <el-input v-model="editForm.name"></el-input>
        </el-form-item>
        <el-form-item label="路径">
          <el-input v-model="editForm.path"></el-input>
        </el-form-item>
        <el-form-item label="序号">
          <el-input v-model="editForm.order" type="number"></el-input>
        </el-form-item>
        <el-form-item label="图标" prop="icon">
          <el-input v-model="editForm.icon"></el-input>
          <i :class="editForm.icon"></i>
        </el-form-item>
        <el-form-item label="根菜单">
          <el-select v-model="editForm.level" placeholder="请选择">
            <el-option
              v-for="item in rootMenuList"
              :key="item.value"
              :label="item.label"
              :value="item.value + ''"
            >
            </el-option>
          </el-select>
        </el-form-item>
      </el-form>
      <span slot="footer" class="dialog-footer">
        <el-button @click="editDialogVisible = false">取 消</el-button>
        <el-button type="primary" @click="editMenu">确 定</el-button>
      </span>
    </el-dialog>
  </div>
</template>

<script>
export default {
  data() {
    var nameExist = async (rule, value, cb) => {
      let id
      if (this.isEdit) {
        id = this.editForm.id
      } else {
        id = 0
      }
      const { data: res } = await this.$http.get(`menus/${id}/exist/${value}`)
      if (res.meta.code !== 200) {
        return this.$message.error('重名认证服务错误')
      }
      if (res.data) {
        return cb(new Error('该菜单名称已被使用'))
      }
      cb()
    }
    return {
      queryInfo: {
        query: '',
        pagenum: 1,
        pagesize: 10
      },
      total: 0,
      menuList: [],
      isEdit: false,
      columns: [
        { label: '菜单名称', prop: 'name' },
        { label: '图标', prop: 'icon' },
        { label: '路径', prop: 'path' },
        { label: '状态', type: 'template', template: 'state' },
        { label: '操作', type: 'template', template: 'opt' }
      ],
      rootMenuList: [],
      addDialogVisible: false,
      addForm: {
        id: 0,
        name: '',
        path: '',
        level: '',
        icon: '',
        order: ''
      },
      addFormRules: {
        name: [
          { required: true, message: '请输入菜单名称', trigger: 'blur' },
          { validator: nameExist, trigger: 'blur' }
        ],
        icon: [{ required: true, message: '请输入图标名称', trigger: 'blur' }]
      },
      editDialogVisible: false,
      editForm: {
        id: 0,
        name: '',
        path: '',
        level: '',
        icon: '',
        order: ''
      },
      editFormRules: {
        name: [
          { required: true, message: '请输入菜单名称', trigger: 'blur' },
          { validator: nameExist, trigger: 'blur' }
        ],
        icon: [{ required: true, message: '请输入图标名称', trigger: 'blur' }]
      }
    }
  },
  created() {
    this.getMenuList()
  },
  methods: {
    async getMenuList() {
      const { data: res } = await this.$http.get('menus', {
        params: this.queryInfo
      })
      if (res.meta.code !== 200) {
        return this.$message.error('获取菜单数据失败')
      }
      this.total = res.data.total
      this.menuList = res.data.result
      console.log(this.menuList)
    },
    async getRootMenuList() {
      const { data: res } = await this.$http.get('/menus/root')
      if (res.meta.code !== 200) {
        return this.$message.error('获取根菜单失败')
      }
      this.rootMenuList = res.data
    },
    handleSizeChange(newSize) {
      this.queryInfo.pagesize = newSize
      this.getMenuList()
    },
    handleCurrentChange(newPage) {
      this.queryInfo.pagenum = newPage
      this.getMenuList()
    },
    showAddDialog() {
      this.getRootMenuList()
      this.addDialogVisible = true
    },
    addMenu() {
      this.$refs.addFormRef.validate(async valid => {
        if (valid) {
          const { data: res } = await this.$http.post('menus', this.addForm)
          if (res.meta.code !== 200) {
            return this.$message.error('添加菜单失败 ' + res.meta.message)
          }

          this.$message.success('添加菜单成功')
          this.addDialogVisible = false
          this.getMenuList()
        }
      })
    },
    async changeMenuState(menu) {
      const { data: res } = await this.$http.put(
        `menus/${menu.id}/state/${menu.state}`
      )
      if (res.meta.code !== 200) {
        menu.state = !menu.state
        return this.$message.error('修改菜单状态失败')
      }
      this.$message.success('修改状态成功')
    },
    async showEditDialog(menu) {
      this.isEdit = true
      const { data: res } = await this.$http.get(`menus/${menu.id}`)
      if (res.meta.code !== 200) {
        return this.$message.error('获取菜单数据失败')
      }
      this.editForm = res.data
      console.log(this.editForm)
      this.getRootMenuList()
      this.editDialogVisible = true
    },
    editMenu() {
      this.$refs.editFormRef.validate(async valid => {
        if (valid) {
          const { data: res } = await this.$http.post('menus', this.editForm)
          if (res.meta.code !== 200) {
            return this.$message.error('修改菜单失败')
          }
          this.$message.success('修改菜单信息成功')
          this.getMenuList()
          this.editDialogVisible = false
        }
      })
    },
    addDialogClosed() {
      this.$refs.addFormRef.resetFields()
    },
    editDialogClosed() {
      this.$refs.editFormRef.resetFields()
    }
  }
}
</script>

<style lang="less" scoped>
.treeTable {
  margin-top: 15px;
}
</style>
