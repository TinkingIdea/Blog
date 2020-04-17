<template>
  <div>
    <!-- 面包屑导航区域 -->
    <el-breadcrumb separator-class="el-icon-arrow-right">
      <el-breadcrumb-item :to="{ path: '/home' }">首页</el-breadcrumb-item>
      <el-breadcrumb-item>分类管理</el-breadcrumb-item>
      <el-breadcrumb-item>分类列表</el-breadcrumb-item>
    </el-breadcrumb>

    <el-card>
      <el-row :gutter="20">
        <el-col :span="8">
          <el-input
            placeholder="请输入内容"
            v-model="queryInfo.query"
            clearable
            @clear="getCateList"
          >
            <el-button
              slot="append"
              icon="el-icon-search"
              @click="getCateList"
            ></el-button>
          </el-input>
        </el-col>
        <el-col :span="4">
          <el-button type="primary" @click="addDialogVisible = true"
            >添加分类</el-button
          >
        </el-col>
      </el-row>

      <el-table :data="cateList" border stripe>
        <el-table-column label="#" type="index"></el-table-column>
        <el-table-column label="分类名称" prop="name"></el-table-column>
        <el-table-column label="状态">
          <template slot-scope="scope">
            <el-switch
              v-model="scope.row.state"
              @change="changeCateState(scope.row)"
            ></el-switch>
          </template>
        </el-table-column>
        <el-table-column label="操作">
          <template slot-scope="scope">
            <el-button
              type="primary"
              size="mini"
              icon="el-icon-edit"
              @click="showEditDialog(scope.row.id)"
              >编辑</el-button
            >
            <el-button
              type="danger"
              size="mini"
              icon="el-icon-delete"
              @click="removeCate(scope.row.id)"
              >删除</el-button
            >
          </template>
        </el-table-column>
      </el-table>
      <el-pagination
        @size-change="handleSizeChange"
        @current-change="handleCurrentChange"
        :current-page="queryInfo.pagenum"
        :page-size="queryInfo.pagesize"
        layout="total, prev, pager, next, jumper"
        :total="total"
        background
      >
      </el-pagination>
    </el-card>

    <el-dialog
      title="添加分类"
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
        <el-form-item label="分类名称" prop="name">
          <el-input v-model="addForm.name"></el-input>
        </el-form-item>
      </el-form>
      <span slot="footer" class="dialog-footer">
        <el-button @click="addDialogVisible = false">取 消</el-button>
        <el-button type="primary" @click="addCate">确 定</el-button>
      </span>
    </el-dialog>

    <el-dialog
      title="修改分类"
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
        <el-form-item label="分类名称" prop="name">
          <el-input v-model="editForm.name"></el-input>
        </el-form-item>
      </el-form>
      <span slot="footer" class="dialog-footer">
        <el-button @click="editDialogVisible = false">取 消</el-button>
        <el-button type="primary" @click="editCate">确 定</el-button>
      </span>
    </el-dialog>
  </div>
</template>

<script>
export default {
  data() {
    var nameExist = async (rule, value, cb) => {
      let id = 0
      if (this.isEdit) {
        id = this.editForm.id
      }
      const { data: res } = await this.$http.get(
        `categories/${id}/exist/${value}`
      )
      if (res.meta.code !== 200) {
        return this.$message.error('重名检验服务错误')
      }
      if (res.data) {
        return cb(new Error('该分类名称已被使用'))
      }
      cb()
    }
    return {
      queryInfo: {
        query: '',
        pagenum: 1,
        pagesize: 10
      },
      cateList: [],
      total: 0,
      addDialogVisible: false,
      isEdit: false,
      addForm: {
        id: 0,
        name: ''
      },
      addFormRules: {
        name: [
          { required: true, message: '请输入分类名称', trigger: 'blur' },
          { validator: nameExist, trigger: 'blur' }
        ]
      },
      editDialogVisible: false,
      editForm: {
        id: 0,
        name: ''
      },
      editFormRules: {
        name: [
          { required: true, message: '请输入分类名称', trigger: 'blur' },
          { validator: nameExist, trigger: 'blur' }
        ]
      }
    }
  },
  created() {
    this.getCateList()
  },
  methods: {
    async getCateList() {
      const { data: res } = await this.$http.get('categories', {
        params: this.queryInfo
      })
      if (res.meta.code !== 200) {
        return this.$message.error('获取分类列表数据失败')
      }

      this.cateList = res.data.result
      this.total = res.data.total
    },
    addCate() {
      this.$refs.addFormRef.validate(async valid => {
        if (valid) {
          const { data: res } = await this.$http.post(
            'categories',
            this.addForm
          )
          if (res.meta.code !== 200) {
            return this.$message.error('添加分类失败')
          }
          this.$message.success('添加分类成功')
          this.getCateList()
          this.addDialogVisible = false
        }
      })
    },
    addDialogClosed() {
      this.$refs.addFormRef.resetFields()
    },
    async showEditDialog(id) {
      const { data: res } = await this.$http.get(`categories/${id}`)
      if (res.meta.code !== 200) {
        return this.$message.error('获取分类信息失败')
      }
      this.editForm = res.data
      this.editDialogVisible = true
    },
    editCate() {
      this.$refs.editFormRef.validate(async valid => {
        if (valid) {
          const { data: res } = await this.$http.post(
            'categories',
            this.editForm
          )
          if (res.meta.code !== 200) {
            return this.$message.error('修改分类失败')
          }
          this.$message.success('修改分类成功')
          this.getCateList()
          this.editDialogVisible = false
        }
      })
    },
    editDialogClosed() {
      this.$refs.editFormRef.resetFields()
    },
    async changeCateState(cate) {
      const { data: res } = await this.$http.get(
        `categories/${cate.id}/state/${cate.state}`
      )
      if (res.meta.code !== 200) {
        cate.state = !cate.state
        return this.$message.error('更新分类状态失败: ' + res.message)
      }
      this.$message.success('更新分类状态成功')
    },
    async removeCate(id) {
      const confirmResult = await this.$confirm(
        '此操作将永久删除该分类, 是否继续?',
        '提示',
        {
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          type: 'warning'
        }
      ).catch(err => err)

      if (confirmResult !== 'confirm') {
        return this.$message.info('已取消删除')
      }

      const { data: res } = await this.$http.delete(`categories/${id}`)
      if (res.meta.code !== 200) {
        return this.$message.error('删除分类失败')
      }
      this.$message.success('删除分类成功')
      this.getCateList()
    },
    handleSizeChange(newSize) {
      this.queryInfo.pagesize = newSize
      this.getCateList()
    },
    handleCurrentChange(newPage) {
      this.queryInfo.pagenum = newPage
      this.getCateList()
    }
  }
}
</script>

<style lang="less" scoped></style>
