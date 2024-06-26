using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.DataFormats;
using Timer = System.Timers.Timer;
using NAudio.CoreAudioApi;
using NAudio.Dsp;
using System.Windows.Forms;
using System.Security.Cryptography.X509Certificates;
using NAudio.CoreAudioApi.Interfaces;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using static UEP.Form1;


namespace UEP
{
    public partial class Form1 : Form
    {
        // 필요한 외부 함수 선언
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetWindowPlacement(IntPtr hWnd, out WINDOWPLACEMENT lpwndpl);
        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool QueryFullProcessImageName([In] IntPtr hProcess, [In] uint dwFlags, [Out] StringBuilder lpExeName, ref uint lpdwSize);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr OpenProcess(uint processAccess, bool bInheritHandle, uint processId);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CloseHandle(IntPtr hObject);

        [DllImport("Shell32.dll", SetLastError = true)]
        static extern int ExtractIconEx(string sFile, int iIndex, out IntPtr piLargeVersion, out IntPtr piSmallVersion, int amountIcons);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool DestroyIcon(IntPtr hIcon);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        delegate bool EnumThreadDelegate(IntPtr hWnd, int lParam);

        [DllImport("user32.dll")]
        private static extern bool EnumThreadWindows(int dwThreadId, EnumThreadDelegate lpfn, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern int ShowCursor(bool bShow);


        [DllImport("user32.dll")]
        static extern bool SetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);


        // 모니터 방향 회전 관련 API
        [DllImport("user32.dll", CharSet = CharSet.Ansi)]
        private static extern int ChangeDisplaySettingsEx(string lpszDeviceName, ref DEVMODE lpDevMode, IntPtr hwnd, uint dwflags, IntPtr lParam);

        private const int ENUM_CURRENT_SETTINGS = -1;
        private const int CDS_UPDATEREGISTRY = 0x00000001;
        private const int CDS_RESET = 0x40000000;
        private const int DISP_CHANGE_SUCCESSFUL = 0;
        private const int DM_DISPLAYORIENTATION = 0x80;
        private const int DM_PELSWIDTH = 0x80000;
        private const int DM_PELSHEIGHT = 0x100000;

        [DllImport("user32.dll")]
        private static extern int EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE devMode);
        [DllImport("gdi32.dll")]
        private static extern bool SetDeviceGammaRamp(IntPtr hdc, short[] ramp);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, ref Rect rect);

        private delegate bool EnumThreadWndProc(IntPtr hWnd, IntPtr lParam);

        private const int GAMMA_RAMP_SIZE = 256;

        private const int PHYSICALWIDTH = 110;
        private const int PHYSICALHEIGHT = 111;

        const uint PROCESS_QUERY_INFORMATION = 0x0400;
        const uint PROCESS_VM_READ = 0x0010;

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WINDOWPLACEMENT
        {
            public int Length;
            public int Flags;
            public int ShowCmd;
            public POINT MinPosition;
            public POINT MaxPosition;
            public RECT NormalPosition;
        }
        // DEVMODE 구조체
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct DEVMODE
        {
            private const int CCHDEVICENAME = 32;
            private const int CCHFORMNAME = 32;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
            public string dmDeviceName;
            public ushort dmSpecVersion;
            public ushort dmDriverVersion;
            public ushort dmSize;
            public ushort dmDriverExtra;
            public int dmFields;

            public int dmPositionX;
            public int dmPositionY;
            public ScreenOrientation dmDisplayOrientation;
            public int dmDisplayFixedOutput;

            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHFORMNAME)]
            public string dmFormName;
            public short dmLogPixels;
            public int dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmNup;
            public int dmDisplayFrequency;
        }

        public enum ScreenOrientation
        {
            DMDO_DEFAULT = 0,
            DMDO_90 = 1,
            DMDO_180 = 2,
            DMDO_270 = 3
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        private const int SW_HIDE = 0;
        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOWMAXIMIZED = 3;
        private const int SW_SHOWNOACTIVATE = 4;

        private DataGridView dataGridView1; // 상단에 프로세스 정보 출력
        private DataGridView dataGridView2; // 하단에 프로세스 정보 출력
        private ImageList imageList1; // 아이콘 삽입을 위한 이미지 리스트
        private TextBox txtProcessName;
        private Button btnSavePath;
        private Button btnRunPath;
        private Button btnProcessRefresh;
        private Button btnDeletePath;
        private Button deletePreset;
        private Button btnRelocation;
        private ComboBox comboBox;

        private TabControl tabControl1;
        private TabPage tabPage1;

        // 마우스 컨트롤 탭에 관련한 초기화
        TrackBar trackBarMouseSpeed;
        TextBox txtSpeedValue;
        CheckBox chkInvertMouse;
        Timer inactivityTimer;
        bool isCursorHidden = false;
        bool enableHideCursor = true;
        Point lastMousePosition;
        CheckBox chkHideCursor;
        TrackBar trackBarWheelSensitivity;
        TextBox txtWheelSensitivityValue;


        private string folderPath;
        private string selectFile;


        public Form1()
        {
            InitializeComponent();
            InitializeComponents();
            InitializeMonitor();
            InitializeVolumeControl();
            InitializeMicVolumeControl();

            // 메인폼의 크기 상태 설정
            this.Size = new Size(1200, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = this.Size;
            this.MaximumSize = this.Size;
            this.AutoScaleMode = AutoScaleMode.None;
            this.AutoSize = true;

            // 모니터 감마조절 함수 가져오기
            this.gamma = new Gamma(Screen.PrimaryScreen.DeviceName);
            this.currentGamma = (float)this.sldValue.Value / 1000f;
            this.tbValue.Text = this.sldValue.Value.ToString();
            this.gamma.Set(this.currentGamma);

        }

        // 디자인 코드
        private void InitializeComponents()
        {
            imageList1 = new ImageList();
            imageList1.ImageSize = new Size(100, 100);  // 아이콘 크기 설정

            this.dataGridView1 = new DataGridView();
            this.dataGridView2 = new DataGridView();

            // 그리드뷰의 크기 변경을 못하도록 하기
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            this.dataGridView1.AllowUserToResizeColumns = true; // 열 크기 조절 허용
            this.dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            this.dataGridView2.AllowUserToResizeRows = false;
            this.dataGridView2.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            this.dataGridView2.AllowUserToResizeColumns = true; // 열 크기 조절 허용
            this.dataGridView2.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            // DataGridView 설정
            this.dataGridView1.Location = new Point(30, 30);
            this.dataGridView1.Size = new Size(1100, 300);
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeColumns = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.ReadOnly = true;

            this.dataGridView2.Size = new Size(1100, 280);
            this.dataGridView2.AllowUserToAddRows = false;
            this.dataGridView2.AllowUserToDeleteRows = false;
            this.dataGridView2.AllowUserToResizeColumns = true;
            this.dataGridView2.RowHeadersVisible = false;
            this.dataGridView2.ReadOnly = true;

            // 컬럼 추가 및 초기 너비 설정
            AddImageColumn(".", 30, 1);
            AddColumn("App Name", 300, 1);
            AddColumn("Process Name", 200, 1);
            AddColumn("X", 100, 1);
            AddColumn("Y", 100, 1);
            AddColumn("Width", 100, 1);
            AddColumn("Height", 100, 1);
            AddColumn("State", 100, 1);
            AddHideColumn("ExePath", 0, 1);
            AddHideColumn("TxtPath", 0, 1);
            AddHideColumn("FolderPath", 0, 1);

            // 컬럼 추가 및 초기 너비 설정
            AddColumn("순서", 50, 2);
            AddImageColumn(".", 30, 2);
            AddColumn("App Name", 300, 2);
            AddColumn("Process Name", 200, 2);
            AddColumn("X", 100, 2);
            AddColumn("Y", 100, 2);
            AddColumn("Width", 100, 2);
            AddColumn("Height", 100, 2);
            AddColumn("State", 100, 2);
            AddHideColumn("ExePath", 0, 2);
            AddHideColumn("TxtPath", 0, 2);
            AddHideColumn("FolderPath", 0, 2);

            // 탭 컨트롤 생성
            tabControl1 = new TabControl();

            // 탭 컨트롤의 위치 및 크기 설정
            tabControl1.Location = new Point(30, 500); // 위치
            tabControl1.Size = new Size(1100, 300); // 크기

            // 탭 컨트롤을 폼에 추가
            this.Controls.Add(tabControl1);

            // 탭 페이지 추가
            TabPage tabPage1 = new TabPage("SoftWare");
            TabPage tabPage2 = new TabPage("Mouse");
            TabPage tabPage3 = new TabPage("Display");
            TabPage tabPage4 = new TabPage("Sound");

            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Controls.Add(tabPage4);

            // 프로세스 로드 (예시: 데이터를 직접 추가)
            LoadProcesses();

            dataGridView1.CellDoubleClick += new DataGridViewCellEventHandler(gridView1_CellClick); // 이거 위치 옮겨도 되는지 확인해보자

            dataGridView2.CellDoubleClick += new DataGridViewCellEventHandler(dataGridView2_CellDoubleClick);

            // 텍스트 박스 설정
            txtProcessName = new TextBox();
            txtProcessName.Location = new Point(30, 350); // 위치 설정
            txtProcessName.Size = new Size(200, 20); // 크기 설정

            // 버튼 설정
            btnSavePath = new Button();
            btnSavePath.Location = new Point(240, 350); // 위치 설정
            btnSavePath.Size = new Size(50, 0); // 크기 설정
            btnSavePath.AutoSize = true;
            btnSavePath.Text = "저장"; // 버튼 텍스트 설정
            btnSavePath.Click += new EventHandler(btnSavePath_Click); // 클릭 이벤트 핸들러 연결

            // 프로세스목록을 새로고침하는 버튼
            btnProcessRefresh = new Button();
            btnProcessRefresh.Location = new Point(1050, 350); // 위치 설정
            btnProcessRefresh.Size = new Size(80, 0); // 크기 설정
            btnProcessRefresh.AutoSize = true;
            btnProcessRefresh.Text = "새로고침"; // 버튼 텍스트 설정
            btnProcessRefresh.Click += new EventHandler(btnProcessRefresh_Click); // 클릭 이벤트 핸들러 연결

            // 버튼 설정            
            btnRunPath = new Button();
            btnRunPath.Location = new Point(240, 450); // 위치 설정
            btnRunPath.Size = new Size(50, 0); // 크기 설정
            btnRunPath.AutoSize = true;
            btnRunPath.Text = "실행"; // 버튼 텍스트 설정
            btnRunPath.Click += new EventHandler(btnRunPath_Click); // 클릭 이벤트 핸들러 연결

            // 프리셋(텍스트파일)을 삭제하는 버튼            
            deletePreset = new Button();
            deletePreset.Location = new Point(310, 450); // 위치 설정
            deletePreset.Size = new Size(80, 0); // 크기 설정
            deletePreset.AutoSize = true;
            deletePreset.Text = "프리셋삭제"; // 버튼 텍스트 설정
            deletePreset.Click += new EventHandler(deletePreset_Click); // 클릭 이벤트 핸들러 연결

            // 그리드뷰2 행 삭제 버튼
            btnDeletePath = new Button();
            btnDeletePath.Location = new Point(30, 800); // 위치 설정
            btnDeletePath.Size = new Size(0, 0); // 크기 설정
            btnDeletePath.AutoSize = true;
            btnDeletePath.Text = "행 삭제"; // 버튼 텍스트 설정
            btnDeletePath.Click += new EventHandler(deleteButton_Click); // 클릭 이벤트 핸들러 연결

            // 창 재배치 버튼
            btnRelocation = new Button();
            btnRelocation.Location = new Point(410, 450); // 위치 설정
            btnRelocation.Size = new Size(80, 0); // 크기 설정
            btnRelocation.AutoSize = true;
            btnRelocation.Text = "창 재배치"; // 버튼 텍스트 설정
            btnRelocation.Click += new EventHandler(btnRelocation_Click); // 클릭 이벤트 핸들러 연결


            // 프리셋 목록 보여주기
            comboBox = new ComboBox();
            comboBox.Location = new Point(30, 450);
            comboBox.Size = new Size(200, 20);
            comboBox.Text = "프리셋 목록";
            comboBox.Click += new EventHandler(LoadTextFilesToComboBox);
            comboBox.SelectedIndexChanged += new EventHandler(comboBox_SelectedIndexChanged);


            // 텍스트 박스와 버튼을 폼에 추가
            this.Controls.Add(txtProcessName);
            this.Controls.Add(btnSavePath);
            this.Controls.Add(btnRunPath);
            this.Controls.Add(comboBox);
            this.Controls.Add(btnProcessRefresh);
            this.Controls.Add(btnDeletePath);
            this.Controls.Add(deletePreset);
            this.Controls.Add(btnRelocation);

            // 그리드뷰2를 첫 번째 탭 페이지에 추가
            tabPage1.Controls.Add(dataGridView2);

            // 폼에 DataGridView 추가
            this.Controls.Add(this.dataGridView1);

            // Mouse tab UI 요소 추가
            InitializeMouseTabComponents(tabPage2);

            // Monitor tab UI 요소 추가
            InitializeMonitorTabComponents(tabPage3);

            // 오디오 ui
            InitializeUIComponents(tabPage4, tabControl1);

        }



        // 그리드뷰에 문자열 추가
        private void AddColumn(string name, int initialWidth, int num)
        {
            var column = new DataGridViewTextBoxColumn();
            column.Name = name;
            column.HeaderText = name;
            column.Width = initialWidth;
            column.MinimumWidth = 50; // 최소 너비 설정
            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None; // 자동 크기 조정 비활성화

            if (num == 1)
            {
                this.dataGridView1.Columns.Add(column);
            }
            else
            {
                this.dataGridView2.Columns.Add(column);
            }
        }


        // 그리드뷰에 숨겨진 문자열 추가
        private void AddHideColumn(string name, int initialWidth, int num)
        {
            var column = new DataGridViewTextBoxColumn();
            column.Name = name;
            column.HeaderText = name;
            column.Width = initialWidth;
            column.MinimumWidth = 50; // 최소 너비 설정
            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None; // 자동 크기 조정 비활성화
            column.Visible = false;
            column.Width = 0;

            if (num == 1)
            {
                this.dataGridView1.Columns.Add(column);
            }
            else
            {
                this.dataGridView2.Columns.Add(column);
            }
        }


        // 그리드뷰에 이미지 추가
        private void AddImageColumn(string name, int initialWidth, int num)
        {
            var column = new DataGridViewImageColumn();
            column.Name = name;
            column.HeaderText = name;
            column.Width = initialWidth;
            column.ImageLayout = DataGridViewImageCellLayout.Zoom; // 이미지 레이아웃 설정

            if (num == 1)
            {
                this.dataGridView1.Columns.Add(column);
            }
            else
            {
                this.dataGridView2.Columns.Add(column);
            }
        }


        // 프리셋 삭제 함수
        private void deletePreset_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dataGridView2.Rows)
                {
                    (row.Cells[1].Value as System.Drawing.Image).Dispose();
                }
                dataGridView2.Rows.Clear();

                // 폴더 내의 모든 파일과 하위 폴더를 삭제
                Directory.Delete(folderPath, true);
                MessageBox.Show("프리셋이 삭제되었습니다.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("프리셋을 선택해주세요");
            }
        }


        // 프로세스를 새로고침 하는 버튼의 이벤트
        private void btnProcessRefresh_Click(object sender, EventArgs e)
        {
            LoadProcesses();
        }


        // 그리드뷰2의 행을 더블클릭 했을때 세부 조정 사항 띄우기
        private void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow selectedRow = dataGridView2.Rows[e.RowIndex];
            Form2 detailsForm = new Form2(selectedRow);
            detailsForm.ShowDialog();
            UpdateOrderColumn();

        }


        // 콤보박스의 메뉴를 선택했을 경우, 경로를 찾고 텍스트파일을 읽어서 프로세스 저장 정보를 가져온다
        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox.SelectedIndex != -1) // 선택된 항목이 있는지 확인합니다.
            {
                string selectedFile1 = comboBox.SelectedItem.ToString();
                string selectedFile = selectedFile1 + ".txt";
                Console.WriteLine(selectedFile);

                string modifiedFile = selectedFile.Substring(0, selectedFile.Length - 4);

                string exeLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string exeDirectory = Path.GetDirectoryName(exeLocation);
                string filePath = Path.Combine(exeDirectory, modifiedFile + "\\" + selectedFile);

                Console.WriteLine(filePath);

                folderPath = Path.Combine(exeDirectory, modifiedFile);
                selectFile = filePath;

                List<string> processInfoLines = new List<string>(); // 프로세스 정보를 저장할 리스트
                List<string> processPathLines = new List<string>(); // 프로세스 실행 경로를 저장할 리스트

                foreach (var line in File.ReadLines(filePath))
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        if (line.Contains("//"))
                        {
                            // 프로세스 정보 라인
                            processInfoLines.Add(line);
                        }
                        else
                        {
                            // 프로세스 실행 경로 라인
                            processPathLines.Add(line);
                        }
                    }
                }

                DisplayProcessInfoInGridView(processInfoLines, modifiedFile); // 데이터를 그리드 뷰에 표시합니다.
            }
        }


        // 콤보박스의 메뉴를 선택했을 경우, 그리드뷰2에 프로세스 정보를 업로드한다
        private void DisplayProcessInfoInGridView(List<string> processInfoLines, string modifiedFile)
        {
            dataGridView2.Rows.Clear(); // 기존의 내용을 지우고 새 정보를 표시합니다.
            int imageNum = 0;
            foreach (string line in processInfoLines)
            {
                // line 변수에 저장된 텍스트를 "//" 기준으로 분할
                string[] data = line.Split(new string[] { "//" }, StringSplitOptions.None);


                // DataGridViewRow 객체 생성
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dataGridView2);


                // 현재 실행 중인 어셈블리의 위치를 얻음
                string exeLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
                // 어셈블리의 디렉터리를 얻음
                string exeDirectory = Path.GetDirectoryName(exeLocation);
                // 이미지 파일의 상대 경로를 추가하여 전체 경로를 구성
                string imagePath = Path.Combine(exeDirectory, modifiedFile, imageNum.ToString() + ".png");
                imageNum++;

                // 이미지 파일을 로드
                System.Drawing.Image icon = System.Drawing.Image.FromFile(imagePath);

                // 로드한 이미지를 DataGridView의 셀에 할당
                row.Cells[1].Value = icon;


                // 나머지 열에 텍스트 데이터 할당
                for (int i = 2; i < row.Cells.Count; i++)
                {
                    // data 배열의 길이를 초과하지 않도록 조건 확인
                    if (i < data.Length + 1)
                    {
                        row.Cells[i].Value = data[i - 1];
                    }
                }

                // 설정된 행을 DataGridView에 추가
                dataGridView2.Rows.Add(row);
                UpdateOrderColumn();
            }

            //PrintGridViewData(dataGridView2); //단순하게 콘솔에 출력해서 확인하는 용도
        }


        // 콤보박스에 지정 디렉토리에 있는 모든 텍스트 파일을 가져오는 함수
        private void LoadTextFilesToComboBox(object sender, EventArgs e)
        {
            comboBox.Items.Clear(); // 기존 목록을 비웁니다.

            // 현재 실행 중인 어셈블리의 위치를 얻음
            string exeLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            // 어셈블리의 디렉터리를 얻음
            string exeDirectory = Path.GetDirectoryName(exeLocation);
            // 이미지 파일의 상대 경로를 추가하여 전체 경로를 구성
            string directoryPath = Path.Combine(exeDirectory);


            // "[FILE]"로 시작하는 모든 폴더를 가져옵니다.
            string[] directories = Directory.GetDirectories(directoryPath, "[FILE]*");
            foreach (string directory in directories)
            {
                // 각 폴더 내의 모든 .txt 파일을 가져옵니다.
                string[] files = Directory.GetFiles(directory, "*.txt");

                //foreach (string file in files)
                //{
                //FileInfo fileInfo = new FileInfo(file);
                string combineDerectory = directory.Substring(directory.IndexOf("[FILE]_"));
                comboBox.Items.Add(combineDerectory); // 파일 이름을 ComboBox에 추가합니다.
                //}
            }
        }


        // 프로세스의 경로를 텍스트 파일에 저장하는 함수 (프로세스 정보는 함수로 뺌)
        public void SaveProcessPathToFile(string filePath, DataGridView gridView2)
        {
            using (StreamWriter sw = new StreamWriter(filePath, false))
            {
                foreach (DataGridViewRow row in gridView2.Rows)
                {
                    if (row.Cells["App Name"].Value != null)
                    {
                        string appName = row.Cells["App Name"].Value.ToString();
                        string processName = row.Cells["Process Name"].Value.ToString();
                        string x = row.Cells["X"].Value.ToString();
                        string y = row.Cells["Y"].Value.ToString();
                        string width = row.Cells["Width"].Value.ToString();
                        string height = row.Cells["Height"].Value.ToString();
                        string state = row.Cells["State"].Value.ToString();
                        string processPath = row.Cells["ExePath"].Value?.ToString(); // 얘가 실행파일 경로

                        sw.WriteLine(processPath); // 실행파일 경로 먼저
                        sw.WriteLine($"//{appName}//{processName}//{x}//{y}//{width}//{height}//{state}//{processPath}"); // 프로세스 정보
                        sw.WriteLine(); // 한칸 띄기
                    }
                }
            }
        }


        // 파일에 아이콘을 저장하는 함수
        static void SaveProcessIconToFile(string folderPath, DataGridView gridView2)
        {
            int imageIndex = 0; // 파일명에 사용될 인덱스입니다.

            foreach (DataGridViewRow row in gridView2.Rows)
            {
                // DataGridView의 첫 번째 열에서 특정 행의 셀 이미지를 가져옵니다.
                var cellValue = row.Cells[1].Value;

                // 셀의 값이 null이 아니고, Image 타입인지 확인합니다.
                if (cellValue != null && cellValue is System.Drawing.Image)
                {
                    // 이미지 파일로 저장합니다.
                    System.Drawing.Image img = (System.Drawing.Image)cellValue;

                    string imagePath = Path.Combine(folderPath, $"{imageIndex}.png"); // PNG 파일로 저장
                    img.Save(imagePath, ImageFormat.Png); // ImageFormat에 따라 다른 형식으로 저장할 수 있습니다.

                    imageIndex++; // 다음 이미지에 대한 인덱스를 증가시킵니다.
                }
                else
                {
                    // 적절한 예외 처리나 기본값 반환
                }
            }
        }


        // 저장 버튼을 누르면 프로세스의 경로가 텍스트 파일 형태로 저장되는 함수
        private void btnSavePath_Click(object sender, EventArgs e)
        {
            if (txtProcessName.Text == "")
            {
                MessageBox.Show("프리셋 이름을 입력해주세요");
                return;
            }
            // 프로세스 행의 색이 분홍색이면 저장하지 않습니다.
            List<string> pinkProcesses = new List<string>();
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                if (row.DefaultCellStyle.BackColor == Color.Pink)
                {
                    string appName = row.Cells["app name"].Value.ToString();
                    pinkProcesses.Add(appName);
                }
            }

            if (pinkProcesses.Count > 0)
            {
                string message = "유효하지 않은 경로의 프로세스 입니다:\n\n";
                foreach (string appName in pinkProcesses)
                {
                    message += appName + "\n";
                }
                MessageBox.Show(message);
                return;
            }

            // 프로세스 이름을 기반으로 폴더 및 파일 이름을 지정합니다.
            string folderName = "[FILE]_" + txtProcessName.Text;
            string fileName = folderName + ".txt";
            string folderPath = Path.Combine(System.Windows.Forms.Application.StartupPath, folderName); // 애플리케이션 실행 경로에 폴더 생성
            string filePath = Path.Combine(folderPath, fileName); // 최종 파일 경로

            // 지정된 경로에 폴더가 없다면 생성합니다.
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }


            // 프로세스 경로를 파일에 저장하는 메서드를 호출합니다.
            SaveProcessPathToFile(filePath, dataGridView2); // 굳이 인자로 넣어야 하나? 나중에 확인
            SaveProcessIconToFile(folderPath, dataGridView2);

            SaveMouseSettings("[MOUSE]_" + fileName, folderName);

            SaveMonitorSettings("[MONITOR]_" + fileName, folderName);

            SaveSoundSettings("[SOUND]_" + fileName, folderName);

            MessageBox.Show("저장이 완료되었습니다.");
        }


        // 창 재배치 버튼 이벤트 핸들러 (창을 재배치한다)
        private void btnRelocation_Click(object sender, EventArgs e)
        {

            if (selectFile == null)
            {
                MessageBox.Show("프리셋을 선택해주세요");
                return;
            }

            using (StreamReader sr = new StreamReader(selectFile))
            {
                string processPath;
                while ((processPath = sr.ReadLine()) != null)
                {
                    if (!string.IsNullOrEmpty(processPath))
                    {

                        string processInfo = sr.ReadLine(); // 프로세스 정보 읽기


                        string[] info = processInfo.Split("//");

                        if (info.Length == 9)
                        {
                            int x = int.Parse(info[3]);
                            int y = int.Parse(info[4]);
                            int width = int.Parse(info[5]);
                            int height = int.Parse(info[6]);
                            string state = info[7]; // 상태 값

                            string name = info[2]; // 프로세스 이름

                            Console.WriteLine(" //x축 : " + x + " //y축 : " + y + " //길이 : " + width + " //높이 : " + height + " //상태 : " + state);


                            Process[] processes = Process.GetProcessesByName(name);
                            Process p = processes[0];

                            List<IntPtr> handles = new List<IntPtr>();

                            foreach (ProcessThread thread in p.Threads)
                            {
                                Console.WriteLine("test : " + p.ProcessName);
                                EnumThreadWindows(thread.Id, (hWnd, lParam) => { handles.Add(hWnd); return true; }, IntPtr.Zero);

                            }

                            var processName = name;
                            var windowHandles = GetWindowHandleByProcessName(name, info[1]); // 애는 중복프로세스도 처리 가능


                            Console.WriteLine("그래서 프로세스 찾음? : " + processName + " 핸들러 : " + windowHandles);

                            if (info[7] == "Min")
                            {
                                ChangeProcessWindowState(windowHandles, 2);
                                return;
                            }
                            else if (info[7] == "Max")
                            {
                                ChangeProcessWindowState(windowHandles, 3);
                                return;
                            }

                            // 프로세스 창 크기 변경
                            SetWindowPos(windowHandles, IntPtr.Zero, x, y, width, height, 0);

                        }
                        sr.ReadLine(); // 빈 줄 건너뛰기
                    }
                }
            }
        }


        // 윈도우의 크기 및 위치를 조절하는 함수
        private void ChangeProcessWindowState(IntPtr windowHandle, int windowState)
        {
            switch (windowState)
            {
                case 2: // Minimize
                    ShowWindow(windowHandle, SW_SHOWMINIMIZED);
                    break;
                case 3: // Maximize
                    ShowWindow(windowHandle, SW_SHOWMAXIMIZED);
                    break;
                default:
                    ShowWindow(windowHandle, SW_SHOWNORMAL);
                    break;
            }
        }


        // 실행버튼을 눌렀을 때 동작하는 이벤트 핸들러
        private async void btnRunPath_Click(object sender, EventArgs e)
        {
            if(selectFile == null)
            {
                MessageBox.Show("프리셋을 선택해주세요");
                return;
            }
            using (StreamReader sr = new StreamReader(selectFile))
            {
                string processPath;
                while ((processPath = sr.ReadLine()) != null)
                {
                    if (!string.IsNullOrEmpty(processPath))
                    {
                        await RunProcess(processPath, sr);
                    }
                }
            }

            ApplyMouseSettings(folderPath);
            ApplyMonitorSettings(folderPath);
            ApplySoundSettings(folderPath);
        }


        // 프로세스를 실행시키는 함수
        private async Task RunProcess(string processPath, StreamReader sr)
        {
            await Task.Run(() =>
            {
                Process process = Process.Start(new ProcessStartInfo
                {
                    FileName = processPath,
                    UseShellExecute = true
                });

                //process.WaitForInputIdle(); // Wait until the process is fully loaded

                string processInfo = sr.ReadLine(); // 프로세스 정보 읽기
                UpdateProcessInfo(process, processInfo); // 창 재배치 작업 수행
                sr.ReadLine(); // 빈 줄 건너뛰기
            });
        }


        // 프로세스이름으로 윈도우 창 핸들 가져오는 함수
        public static IntPtr GetWindowHandleByProcessName(string processName, string appName)
        {
            Console.WriteLine("프로세스이름: " + processName + "앱이름 : " + appName);
            List<IntPtr> matchedHandles = new List<IntPtr>();

            foreach (Process p in Process.GetProcessesByName(processName))
            {
                string currentProcessName = p.ProcessName;
                if (currentProcessName.Equals(processName, StringComparison.OrdinalIgnoreCase))
                {
                    foreach (ProcessThread thread in p.Threads)
                    {
                        EnumThreadWindows(thread.Id, (hWnd, lParam) =>
                        {
                            uint processId;
                            GetWindowThreadProcessId(hWnd, out processId);
                            if (processId == (uint)p.Id)
                            {
                                StringBuilder windowText = new StringBuilder(256);
                                GetWindowText(hWnd, windowText, windowText.Capacity);
                                if (windowText.ToString().Contains(appName, StringComparison.OrdinalIgnoreCase))
                                {
                                    matchedHandles.Add(hWnd);
                                }
                            }
                            return true;
                        }, IntPtr.Zero);
                    }
                }
            }

            if (matchedHandles.Count == 1)
            {
                return matchedHandles[0];
            }
            else if (matchedHandles.Count > 1)
            {
                // 프로세스 이름과 애플리케이션 이름이 모두 일치하는 창 핸들 반환
                return matchedHandles.FirstOrDefault();
            }
            else
            {
                return IntPtr.Zero;
            }
        }


        // 프로그램 실행 후, 윈도우 창의 위치, 크기, 상태를 조절하는 함수
        private void UpdateProcessInfo(Process process, string processInfo)
        {
            if (process != null)
            {
                string[] info = processInfo.Split("//");

                if (info.Length == 9)
                {
                    int x = int.Parse(info[3]);
                    int y = int.Parse(info[4]);
                    int width = int.Parse(info[5]);
                    int height = int.Parse(info[6]);
                    string state = info[7]; // 상태 값
                    string name = info[2]; // 프로세스 이름

                    Console.WriteLine(" //x축 : " + x + " //y축 : " + y + " //길이 : " + width + " //높이 : " + height + " //상태 : " + state);

                    Task.Run(() =>
                    {
                        Thread.Sleep(1000);
                        while (true)
                        {
                        
                        // 현재 창 위치 및 크기 가져오기
                        Rect rect = new Rect();
                            IntPtr windowHandle = GetWindowHandleByProcessName(name, info[1]);
                            GetWindowRect(windowHandle, ref rect);
                            Console.WriteLine("++가져온거 : name:"+name+"{0}/{1}/{2}/{3}", rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
                            Console.WriteLine("--저장된거 : name:" + name + "{0}/{1}/{2}/{3}", x, y, width, height);
                            if (rect.Left != x || rect.Top != y || rect.Right - rect.Left != width || rect.Bottom - rect.Top != height)
                            {
                                Console.WriteLine("++가져온거 : name:" + name + "{0}/{1}/{2}/{3}", rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
                                Console.WriteLine("--저장된거 : name:" + name + "{0}/{1}/{2}/{3}", x, y, width, height);

                                // 창 크기 및 위치가 일치하지 않으면 재배치
                                SetWindowPos(windowHandle, IntPtr.Zero, x, y, width, height, 0);
                            }

                            else
                            {

                                Console.WriteLine("=====종료 : name:" + name + "{0}/{1}/{2}/{3}", rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
                                Console.WriteLine("=====종료 : name:" + name + "{0}/{1}/{2}/{3}", x, y, width, height);
                                // 창 크기 및 위치가 일치하면 루프 종료
                                SetWindowPos(windowHandle, IntPtr.Zero, x, y, width, height, 0);
                                break;
                            }

                            Thread.Sleep(500); // 잠시 대기 후 다시 비교
                        }
                        IntPtr windowHandle1 = GetWindowHandleByProcessName(name, info[1]);
                        if (state == "Min")
                        {
                            ChangeProcessWindowState(windowHandle1, 2);
                        }
                        else if (state == "Max")
                        {
                            ChangeProcessWindowState(windowHandle1, 3);
                        }
                    });
                }
            }
        }


        // 1번 뷰의 행을 더블클릭 하면 2번 뷰로 정보를 옮기는 함수
        private void gridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // 유효한 행 인덱스인지 확인합니다.
            {
                // 첫 번째 그리드 뷰의 선택된 행을 가져옵니다.
                DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];

                // 두 번째 그리드 뷰에 동일한 행이 있는지 확인합니다.
                bool isDuplicate = false;
                foreach (DataGridViewRow row in dataGridView2.Rows)
                {
                    bool allCellsMatch = true;
                    // 그리드뷰1의 셀을 순회하면서 그리드뷰2의 인덱스 값과 비교합니다.
                    // 여기서 i는 그리드뷰1의 인덱스, i+1은 그리드뷰2의 인덱스에 해당합니다.
                    for (int i = 0; i < 3; i++)
                    {
                        // 그리드뷰2의 셀 인덱스는 그리드뷰1의 셀 인덱스보다 하나 더 높습니다.
                        int gridView2Index = i + 1;

                        // 해당 열의 값이 다르면, 이 행은 중복되지 않습니다.
                        if (row.Cells[gridView2Index].Value?.ToString() != selectedRow.Cells[i].Value?.ToString())
                        {
                            allCellsMatch = false;
                            break;
                        }
                    }

                    if (allCellsMatch)
                    {
                        // 모든 셀이 일치하면, 중복된 행입니다.
                        isDuplicate = true;
                        break;
                    }
                }

                // 중복되지 않은 경우에만 새 행을 추가합니다.
                if (!isDuplicate)
                {
                    int newRow = dataGridView2.Rows.Add();
                    foreach (DataGridViewCell cell in selectedRow.Cells)
                    {
                        dataGridView2.Rows[newRow].Cells[cell.ColumnIndex + 1].Value = cell.Value; // +1은 순서 열을 고려한 것
                    }

                    // 'App Name'이 현재 실행 중인 프로세스의 'WindowTitle'과 일치하는지 확인합니다.
                    string appName = selectedRow.Cells["App Name"].Value.ToString();
                    bool isAppNameMatched = false;

                    // 현재 실행 중인 모든 프로세스를 검사합니다.
                    foreach (Process process in Process.GetProcesses())
                    {
                        try
                        {
                            if (process.MainWindowTitle.Contains(appName) && process.MainModule.FileName != null)
                            {
                                isAppNameMatched = true;
                                break;
                            }
                        }
                        catch
                        {
                            // 액세스 거부와 같은 예외를 처리합니다.
                        }
                    }
                    UpdateOrderColumn();
                }
            }
        }


        // 삭제버튼을 누르면 그리드뷰2의 행을 삭제하는 함수
        private void deleteButton_Click(object sender, EventArgs e)
        {
            // 선택된 셀이 있는지 확인합니다.
            if (dataGridView2.SelectedCells.Count > 0)
            {
                // 삭제할 행을 보관할 리스트를 생성합니다.
                List<DataGridViewRow> rowsToDelete = new List<DataGridViewRow>();

                // 선택된 셀의 행을 리스트에 추가합니다 (새 행은 제외).
                foreach (DataGridViewCell cell in dataGridView2.SelectedCells)
                {
                    DataGridViewRow row = cell.OwningRow;
                    if (!row.IsNewRow && !rowsToDelete.Contains(row))
                    {
                        rowsToDelete.Add(row);
                    }
                }

                // 리스트에 있는 행들을 삭제합니다.
                foreach (DataGridViewRow row in rowsToDelete)
                {
                    dataGridView2.Rows.Remove(row);
                }

                // 행을 삭제한 후 순서 열을 업데이트합니다.
                UpdateOrderColumn();
            }
            else
            {
                MessageBox.Show("삭제할 행의 셀을 선택해주세요.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        // 추가된 행의 순서를 설정하고 정렬하는 함수
        private void UpdateOrderColumn()
        {
            int order = 1;
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                if (!row.IsNewRow) // 새 행이 아닌 경우에만 순서를 업데이트합니다.
                {
                    row.Cells["순서"].Value = order++;
                }
            }
        }


        // 그리드뷰1에 아이콘을 삽입하기 위해 프로세스에서 아이콘을 가져오는 함수
        private void SaveIconFromWindow(IntPtr hWnd, string windowTitle)
        {
            uint processId;
            GetWindowThreadProcessId(hWnd, out processId);

            IntPtr processHandle = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, false, processId);

            if (processHandle != IntPtr.Zero)
            {
                StringBuilder buffer = new StringBuilder(1024);
                uint bufferSize = (uint)buffer.Capacity + 1; // 버퍼 크기 수정
                if (QueryFullProcessImageName(processHandle, 0, buffer, ref bufferSize))
                {
                    string processPath = buffer.ToString();
                    ExtractAndAddIconToImageList(processPath, windowTitle);
                }
                CloseHandle(processHandle);
            }
        }


        // 그리드뷰1에 아이콘을 삽입하기 위해 이미지리스트에 아이콘 이미지를 넣는 함수
        private void ExtractAndAddIconToImageList(string filePath, string windowTitle)
        {
            IntPtr largeIcon, smallIcon;
            if (ExtractIconEx(filePath, 0, out largeIcon, out smallIcon, 1) > 0)
            {
                using (Icon icon = Icon.FromHandle(largeIcon))
                {
                    // Icon을 ImageList에 추가
                    imageList1.Images.Add(icon.ToBitmap());
                }

                // 아이콘 핸들 정리
                if (largeIcon != IntPtr.Zero) DestroyIcon(largeIcon);
                if (smallIcon != IntPtr.Zero) DestroyIcon(smallIcon);
            }
            else
            {
                // 아이콘 파일이 존재하지 않는 경우 기본 아이콘 사용
                using (Icon defaultIcon = SystemIcons.Application)
                {
                    imageList1.Images.Add(defaultIcon.ToBitmap());
                }
            }
        }


        // 그리드뷰1에 프로세스 정보를 가져오는 함수
        private bool EnumWindowsCallback(IntPtr hWnd, IntPtr lParam)
        {
            if (IsWindowVisible(hWnd))
            {
                StringBuilder sb = new StringBuilder(256);
                if (GetWindowText(hWnd, sb, sb.Capacity) > 0)
                {
                    RECT rect;
                    GetWindowRect(hWnd, out rect);

                    WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
                    placement.Length = Marshal.SizeOf(placement);
                    GetWindowPlacement(hWnd, out placement);

                    string windowState = placement.ShowCmd switch
                    {
                        1 => "Normal",
                        2 => "Min",
                        3 => "Max",
                        _ => "Unknown"
                    };

                    // 창 상태가 최소화일 때 좌표값을 설정 (노말 상태일때의 상태를 가져옴)
                    if (windowState == "Min")
                    {

                        rect.Left = placement.NormalPosition.Left;
                        rect.Top = placement.NormalPosition.Top;
                        rect.Right = placement.NormalPosition.Right;
                        rect.Bottom = placement.NormalPosition.Bottom;
                    }

                    // 프로세스 ID 가져오기
                    GetWindowThreadProcessId(hWnd, out int processId);

                    // 프로세스 이름 가져오기
                    string processName = "Unknown";
                    try
                    {
                        Process process = Process.GetProcessById(processId);
                        processName = process.ProcessName;

                    }
                    catch (Exception ex)
                    {
                        processName = $"Error: {ex.Message}";
                    }

                    string processPath = "Unknown";
                    try
                    {
                        Process process = Process.GetProcessById(processId);
                        processPath = process.MainModule.FileName;
                    }
                    catch (Exception ex)
                    {
                        processPath = $"Error: {ex.Message}";
                    }

                    SaveIconFromWindow(hWnd, processName);

                    var row = new DataGridViewRow();
                    row.CreateCells(dataGridView1);

                    int imageIndex;
                    imageIndex = imageList1.Images.Count - 1;
                    row.Cells[0].Value = imageList1.Images[imageIndex];
                    row.Cells[1].Value = sb;
                    row.Cells[2].Value = processName;
                    row.Cells[3].Value = rect.Left;
                    row.Cells[4].Value = rect.Top;
                    row.Cells[5].Value = rect.Right - rect.Left;
                    row.Cells[6].Value = rect.Bottom - rect.Top;
                    row.Cells[7].Value = windowState;
                    row.Cells[8].Value = processPath; // 실행파일 경로 넣기
                    row.Cells[9].Value = "텍스트경로"; // 실행파일 경로 넣기
                    row.Cells[10].Value = "폴더경로"; // 실행파일 경로 넣기

                    dataGridView1.Rows.Add(row);

                    //Console.WriteLine($"Handle: {hWnd}, Title: {sb}, Process Name: {processName}, X: {rect.Left}, Y: {rect.Top}, Width: {rect.Right - rect.Left}, Height: {rect.Bottom - rect.Top}, State: {windowState}");
                }
            }
            return true; // Return true to continue enumerating the next window
        }


        // 최초에 프로그램을 시작하면 그리드뷰1에 정보를 가져오는 함수
        private void LoadProcesses()
        {
            dataGridView1.Rows.Clear(); // DataGridView 내용을 클리어
            EnumWindows(new EnumWindowsProc(EnumWindowsCallback), IntPtr.Zero);
        }














        // 마우스 세팅값을 저장하는 함수
        private void SaveMouseSettings(string filePath, string folderPath)
        {
            try
            {
                string fullPath = Path.Combine(Path.GetDirectoryName(filePath), folderPath);

                using (StreamWriter writer = new StreamWriter(Path.Combine(fullPath, Path.GetFileName(filePath))))
                {
                    if (trackBarMouseSpeed != null)
                    {
                        writer.WriteLine($"MouseSpeed={trackBarMouseSpeed.Value}");
                    }
                    else
                    {
                        MessageBox.Show("trackBarMouseSpeed is null");
                    }

                    if (trackBarWheelSensitivity != null)
                    {
                        writer.WriteLine($"WheelSensitivity={trackBarWheelSensitivity.Value}");
                    }
                    else
                    {
                        MessageBox.Show("trackBarWheelSensitivity is null");
                    }

                    if (chkInvertMouse != null)
                    {
                        writer.WriteLine($"InvertMouse={chkInvertMouse.Checked}");
                    }
                    else
                    {
                        MessageBox.Show("chkInvertMouse is null");
                    }

                    if (chkHideCursor != null)
                    {
                        writer.WriteLine($"HideCursor={chkHideCursor.Checked}");
                    }
                    else
                    {
                        MessageBox.Show("chkHideCursor is null");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"설정 저장 중 오류 발생: {ex.Message}");
            }
        }

        // 마우스 세팅값을 가져와서 적용시키는 함수
        public void ApplyMouseSettings(string filePath)
        {

            // 마지막 부분 추출
            string fileName = Path.GetFileName(filePath);
            int underscoreIndex = fileName.IndexOf("_");
            string desiredPart = fileName.Substring(fileName.IndexOf("[FILE]")); // 폴더이름 추출

            string fullPath = filePath + "\\[MOUSE]_" + desiredPart + ".txt";

            // 파일 존재 여부 확인
            if (File.Exists(fullPath))
            {
                // 파일 읽기
                string[] lines = File.ReadAllLines(fullPath);

                // 각 설정 값 적용
                foreach (string line in lines)
                {
                    string[] parts = line.Split('=');
                    if (parts.Length == 2)
                    {
                        string setting = parts[0];
                        string value = parts[1];

                        switch (setting)
                        {
                            case "MouseSpeed":
                                trackBarMouseSpeed.Value = int.Parse(value);
                                break;
                            case "WheelSensitivity":
                                trackBarWheelSensitivity.Value = int.Parse(value);
                                break;
                            case "InvertMouse":
                                chkInvertMouse.Checked = bool.Parse(value);
                                break;
                            case "HideCursor":
                                chkHideCursor.Checked = bool.Parse(value);
                                break;
                        }
                    }
                }
            }
            else
            {
                // 파일이 존재하지 않는 경우 기본값 설정
                trackBarMouseSpeed.Value = 1;
                trackBarWheelSensitivity.Value = 1;
                chkInvertMouse.Checked = false;
                chkHideCursor.Checked = false;
                Console.WriteLine("파일이 존재하지 않습니다.");
            }
        }
        public void ApplyMonitorSettings(string filePath)
        {
            // 파일 경로에서 파일 이름 추출
            string fileName = Path.GetFileName(filePath);

            // 파일 이름에서 폴더 이름 추출
            string desiredPart = fileName.Substring(fileName.IndexOf("[FILE]"));

            // 최종 파일 경로 설정
            string fullPath = filePath + "\\[MONITOR]_" + desiredPart + ".txt";
            // 파일 존재 여부 확인
            if (File.Exists(fullPath))
            {
                // 파일 읽기
                string[] lines = File.ReadAllLines(fullPath);
                foreach (string line in lines)
                {

                    // 설정 값 적용
                    string[] parts = line.Split('=');
                    if (parts.Length == 2)
                    {
                        string setting = parts[0].Trim(); // 설정 항목 이름
                        string value = parts[1].Trim();   // 설정 값
                        // 각 설정 항목에 따라 UI 컨트롤에 적용
                        switch (setting)
                        {
                            case "trackBarBrightness":
                                trackBarBrightness.Value = int.Parse(value);
                                break;
                            case "sldValue":
                                sldValue.Value = int.Parse(value);
                                tbValue.Text = value;
                                break;
                            case "comboOrientation":
                                comboOrientation.SelectedIndex = int.Parse(value);
                                break;
                                // 추가적인 설정 항목이 있다면 여기에 추가
                        }
                    }
                }
            }
            else
            {
                // 파일이 존재하지 않는 경우 기본값 설정
                trackBarBrightness.Value = 50; // 예시 기본값
                comboOrientation.SelectedIndex = 0; // 예시 기본값
                Console.WriteLine("모니터 세팅 파일이 존재하지 않습니다.");
            }
        }

        public void ApplySoundSettings(string filePath)
        {
            // 파일 경로에서 파일 이름 추출
            string fileName = Path.GetFileName(filePath);

            // 파일 이름에서 폴더 이름 추출
            string desiredPart = fileName.Substring(fileName.IndexOf("[FILE]"));

            // 최종 파일 경로 설정
            string fullPath = filePath + "\\[SOUND]_" + desiredPart + ".txt";

            // 파일 존재 여부 확인
            if (File.Exists(fullPath))
            {
                // 파일 읽기
                string[] lines = File.ReadAllLines(fullPath);
                foreach (string line in lines)
                {

                    // 설정 값 적용
                    string[] parts = line.Split('=');
                    if (parts.Length == 2)
                    {
                        string setting = parts[0].Trim(); // 설정 항목 이름
                        string value = parts[1].Trim();   // 설정 값

                        // 설정 항목에 따라 UI 컨트롤에 값 적용
                        switch (setting)
                        {
                            case "trackBarVolume":
                                trackBarVolume.Value = int.Parse(value);
                                float Avolume = trackBarVolume.Value / 100.0f;
                                renderDevice.AudioEndpointVolume.MasterVolumeLevelScalar = Avolume;
                                labelVolume.Text = $"Volume: {trackBarVolume.Value}%";
                                break;
                            case "trackBarMicVolume":
                                trackBarMicVolume.Value = int.Parse(value);
                                float Rvolume = trackBarMicVolume.Value / 100.0f;
                                captureDevice.AudioEndpointVolume.MasterVolumeLevelScalar = Rvolume;
                                labelMicVolume.Text = $"Mic Volume: {trackBarMicVolume.Value}%";
                                break;
                                // 추가적인 설정 항목이 있다면 여기에 추가
                        }
                    }
                }
            }
            else
            {
                // 파일이 존재하지 않는 경우 기본값 설정
                trackBarVolume.Value = 30; // 예시 기본값
                trackBarMicVolume.Value = 100; // 예시 기본값
                Console.WriteLine("사운드 세팅 파일이 존재하지 않습니다.");
            }
        }

        private void InitializeMouseTabComponents(TabPage tabPageMouse) // 전체적인 마우스 탭 관리
        {
            // 마우스 속도 조절 관련
            System.Windows.Forms.Label lblMouseSpeed = new System.Windows.Forms.Label();
            lblMouseSpeed.Text = "마우스 속도 조절:";
            lblMouseSpeed.Location = new Point(20, 20);     // 위치
            lblMouseSpeed.Size = new Size(120, 20);         // 크기

            trackBarMouseSpeed = new TrackBar();
            trackBarMouseSpeed.Location = new Point(150, 20); // 위치
            trackBarMouseSpeed.Size = new Size(250, 45);        // 크기
            trackBarMouseSpeed.Minimum = 1;
            trackBarMouseSpeed.Maximum = 20;
            trackBarMouseSpeed.TickFrequency = 1;
            trackBarMouseSpeed.ValueChanged += TrackBarMouseSpeed_ValueChanged;

            // 텍스트 박스로 트랙바 값 표시
            txtSpeedValue = new TextBox();
            txtSpeedValue.ReadOnly = true; // 읽기 전용으로 설정하여 사용자가 텍스트를 편집하지 못하도록 함
            txtSpeedValue.Location = new Point(410, 20);
            txtSpeedValue.Size = new Size(20, 20);
            txtSpeedValue.TextAlign = HorizontalAlignment.Center;

            // 현재 마우스 속도를 불러와 설정
            int currentMouseSpeed = GetMouseSpeed();
            trackBarMouseSpeed.Value = currentMouseSpeed;
            txtSpeedValue.Text = currentMouseSpeed.ToString();

            // 마우스 버튼 반전 기능 
            chkInvertMouse = new CheckBox();
            chkInvertMouse.Text = "마우스 버튼 반전";
            chkInvertMouse.Location = new Point(20, 60);
            chkInvertMouse.Size = new Size(150, 20);
            chkInvertMouse.CheckedChanged += new EventHandler(InvertMouse);

            // 마우스 가만히 두면 커서 없어지는 기능 
            chkHideCursor = new CheckBox
            {
                Text = "커서 숨김",
                Location = new Point(20, 100),
                Size = new Size(100, 20)
            };
            chkHideCursor.CheckedChanged += HideCursor;
            // Initialize timer (1.5초 가만히 있으면 사라짐)
            inactivityTimer = new Timer(1500); // 1.5 seconds
            inactivityTimer.Elapsed += OnInactivityTimerElapsed;

            // Set up mouse hook (마우스 움직임 감지)
            HookManager.MouseMove += HookManager_MouseMove;
            HookManager.Start();

            inactivityTimer.Start();
            // 마우스 휠 감도 조절 기능 
            System.Windows.Forms.Label lblWheelSensitivity = new System.Windows.Forms.Label();
            lblWheelSensitivity.Text = "휠 감도:";
            lblWheelSensitivity.Location = new Point(20, 140);
            lblWheelSensitivity.Size = new Size(100, 20);

            trackBarWheelSensitivity = new TrackBar();
            trackBarWheelSensitivity.Location = new Point(130, 140);
            trackBarWheelSensitivity.Size = new Size(250, 45);
            trackBarWheelSensitivity.Minimum = 1;
            trackBarWheelSensitivity.Maximum = 100;
            trackBarWheelSensitivity.TickFrequency = 1;
            int initialScrollLines = GetMouseWheelScrollLines(); // GetMouseWheelScrollLines 함수를 호출하여 현재 설정된 값을 가져옵니다.
                                                                 // 초기 값이 최소값과 최대값 사이에 있는지 확인하고 설정합니다.
            trackBarWheelSensitivity.Value = Math.Clamp(initialScrollLines, trackBarWheelSensitivity.Minimum, trackBarWheelSensitivity.Maximum);
            trackBarWheelSensitivity.ValueChanged += TrackBarWheelSensitivity_ValueChanged;


            // TextBox to display the current wheel sensitivity value
            txtWheelSensitivityValue = new TextBox();
            txtWheelSensitivityValue.Location = new Point(410, 140);
            txtWheelSensitivityValue.Size = new Size(50, 20);
            txtWheelSensitivityValue.ReadOnly = true; // Make the TextBox read-only
            txtWheelSensitivityValue.TextAlign = HorizontalAlignment.Center;

            // 현재 휠 감도를 불러와 설정
            int currentWheelSensitivity = GetWheelSensitivity();
            trackBarWheelSensitivity.Value = currentWheelSensitivity;
            txtWheelSensitivityValue.Text = currentWheelSensitivity.ToString();

            // UI 요소를 탭 페이지에 추가
            tabPageMouse.Controls.Add(lblMouseSpeed);
            tabPageMouse.Controls.Add(trackBarMouseSpeed);
            tabPageMouse.Controls.Add(txtSpeedValue);
            tabPageMouse.Controls.Add(chkInvertMouse);
            tabPageMouse.Controls.Add(chkHideCursor);
            tabPageMouse.Controls.Add(lblWheelSensitivity);
            tabPageMouse.Controls.Add(trackBarWheelSensitivity);
            tabPageMouse.Controls.Add(txtWheelSensitivityValue);
        }

        // 커서를 숨기는 함수
        private void HideCursor(object sender, EventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            if (chk != null)
            {
                enableHideCursor = chk.Checked;
                if (!enableHideCursor && isCursorHidden)
                {
                    ShowCursor(true);
                    isCursorHidden = false;
                }
            }
        }

        // 마우스 휠 감도 값을 전달하는 함수
        private void TrackBarWheelSensitivity_ValueChanged(object sender, EventArgs e)
        {
            int sensitivity = trackBarWheelSensitivity.Value;
            SetWheelSensitivity(sensitivity);
            txtWheelSensitivityValue.Text = sensitivity.ToString();
        }

        // Windows API를 가져와 마우스 휠 감도를 조절하는 함수
        private void SetWheelSensitivity(int sensitivity)
        {
            SystemParametersInfo(SPI_SETWHEELSCROLLLINES, (uint)sensitivity, IntPtr.Zero, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
        }

        // 마우스 휠 감도를 가져오는 함수 (아마도)
        private int GetMouseWheelScrollLines()
        {
            SystemParametersInfo(SPI_GETWHEELSCROLLLINES, 0, IntPtr.Zero, 0, out uint lines);
            return Math.Max((int)lines, trackBarWheelSensitivity.Minimum); // 범위 안의 값이 되도록 보장
        }

        // 마우스 속도의 값을 전달하는 함수
        private void TrackBarMouseSpeed_ValueChanged(object sender, EventArgs e)
        {
            TrackBar trackBar = sender as TrackBar;
            if (trackBar != null)
            {
                // 진행 막대 바 값 업데이트
                txtSpeedValue.Text = trackBar.Value.ToString();

                // 마우스 속도 조절
                MouseControl mouseControl = new MouseControl();
                mouseControl.SetMouseSpeed(trackBar.Value);
            }
        }

        // 커서를 숨기는데 필요한 함수
        private void OnInactivityTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (enableHideCursor && !isCursorHidden)
            {
                if (Cursor.Position == lastMousePosition)
                {
                    Invoke(new Action(() =>
                    {
                        ShowCursor(false);
                        isCursorHidden = true;
                    }));
                }
                else
                {
                    lastMousePosition = Cursor.Position;
                }
            }
        }

        // 커서를 숨기는데 움직임을 감지하는 함수
        private void HookManager_MouseMove(object sender, MouseEventArgs e)
        {
            if (isCursorHidden)
            {
                ShowCursor(true);
                isCursorHidden = false;
            }
            inactivityTimer.Stop();
            inactivityTimer.Start();
        }

        // 커서를 숨기는 훅 이벤트를 종료하는 함수 (프로그램 종료할 때 같이 꺼짐)
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            HookManager.MouseMove -= HookManager_MouseMove;
            HookManager.Stop();
            inactivityTimer.Dispose();
        }

        // 마우스 버튼 반전 함수
        private void InvertMouse(object sender, EventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            if (chk != null)
            {
                bool isSwapped = chk.Checked;
                SystemParametersInfo(SPI_SETMOUSEBUTTONSWAP, (uint)(isSwapped ? 1 : 0), IntPtr.Zero, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
            }
        }


        private const uint SPI_SETWHEELSCROLLLINES = 0x0069;
        private const uint SPI_GETWHEELSCROLLLINES = 0x0068;
        private const uint SPI_SETMOUSEBUTTONSWAP = 0x0021;
        private const uint SPIF_UPDATEINIFILE = 0x01;
        private const uint SPIF_SENDCHANGE = 0x02;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, IntPtr pvParam, uint fWinIni);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, IntPtr pvParam, uint fWinIni, out uint pvParamOut);



        public static int GetMouseSpeed()
        {
            const uint SPI_GETMOUSESPEED = 0x0070;
            const int MOUSE_SPEED_DEFAULT = 10;

            IntPtr mouseSpeedPtr = Marshal.AllocCoTaskMem(sizeof(int));
            Marshal.WriteInt32(mouseSpeedPtr, MOUSE_SPEED_DEFAULT);

            bool success = SystemParametersInfo(SPI_GETMOUSESPEED, 0, mouseSpeedPtr, 0);
            int mouseSpeed = Marshal.ReadInt32(mouseSpeedPtr);

            Marshal.FreeCoTaskMem(mouseSpeedPtr);

            if (!success)
            {
                MessageBox.Show("마우스 속도를 불러올 수 없습니다.");
                return MOUSE_SPEED_DEFAULT;
            }

            return mouseSpeed;
        }

        public static int GetWheelSensitivity()
        {
            uint wheelSensitivity = 3; // 기본값
            IntPtr pWheelSensitivity = Marshal.AllocHGlobal(sizeof(uint));
            try
            {
                if (!SystemParametersInfo(SPI_GETWHEELSCROLLLINES, 0, pWheelSensitivity, 0))
                {
                    MessageBox.Show("휠 감도를 불러올 수 없습니다.");
                }
                wheelSensitivity = (uint)Marshal.ReadInt32(pWheelSensitivity);
            }
            finally
            {
                Marshal.FreeHGlobal(pWheelSensitivity);
            }
            return (int)wheelSensitivity;
        }

        // 마우스 컨트롤이라고 새로 만들었는데 쓰긴 쓰는데 최적화 할려면 할 수 있음
        public class MouseControl
        {
            // SystemParametersInfo 함수를 호출하기 위한 상수 및 DLLImport 선언
            private const uint SPI_GETMOUSESPEED = 0x0070;
            private const uint SPI_SETMOUSESPEED = 0x0071;
            private const uint SPI_SETMOUSEBUTTONSWAP = 0x0021;
            private const uint SPI_GETWHEELSCROLLLINES = 0x0068;
            private const uint SPI_SETWHEELSCROLLLINES = 0x0069;
            private const uint SPIF_UPDATEINIFILE = 0x01;
            private const uint SPIF_SENDCHANGE = 0x02;

            [DllImport("user32.dll", SetLastError = true)]
            private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, int pvParam, uint fWinIni);

            public void SetMouseSpeed(int speed)
            {
                // SystemParametersInfo 함수 호출하여 마우스 속도 설정 변경
                if (!SystemParametersInfo(SPI_SETMOUSESPEED, 0, speed, SPIF_SENDCHANGE))
                {
                    MessageBox.Show("마우스 속도를 변경할 수 없습니다.");
                }
            }
        }



        ///////////////////////////////////////모니터설정/////////////////////////////////////////

        // DDC/CI 인터페이스
        [DllImport("dxva2.dll", EntryPoint = "GetMonitorBrightness")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetMonitorBrightness(IntPtr hMonitor, out uint pdwMinimumBrightness, out uint pdwCurrentBrightness, out uint pdwMaximumBrightness);

        [DllImport("dxva2.dll", EntryPoint = "SetMonitorBrightness")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetMonitorBrightness(IntPtr hMonitor, uint dwNewBrightness);

        [DllImport("user32.dll", EntryPoint = "MonitorFromWindow")]
        public static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

        [DllImport("dxva2.dll", EntryPoint = "GetNumberOfPhysicalMonitorsFromHMONITOR")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetNumberOfPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, ref uint pdwNumberOfPhysicalMonitors);

        [DllImport("dxva2.dll", EntryPoint = "GetPhysicalMonitorsFromHMONITOR")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, uint dwPhysicalMonitorArraySize, [Out] PHYSICAL_MONITOR[] pPhysicalMonitorArray);

        [DllImport("dxva2.dll", EntryPoint = "DestroyPhysicalMonitors")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyPhysicalMonitors(uint dwPhysicalMonitorArraySize, [In] PHYSICAL_MONITOR[] pPhysicalMonitorArray);

        private bool isFirstOrientationChange = true;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct PHYSICAL_MONITOR
        {
            public IntPtr hPhysicalMonitor;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szPhysicalMonitorDescription;
        }

        // 모니터 설정에 필요한 선언
        private const float DEF = 1f;
        private Gamma gamma;
        private float currentGamma;
        private TrackBar sldValue;
        private TextBox tbValue;
        private ScreenOrientation currentOrientation;
        private ComboBox comboOrientation;
        private TrackBar trackBarBrightness;
        private TextBox txtBrightnessValue;
        private PHYSICAL_MONITOR[] physicalMonitors;
        private uint minBrightness, currentBrightness, maxBrightness;
        private Button resetMonitor;


        // 모니터 탭 관리
        private void InitializeMonitorTabComponents(TabPage tabPageMonitor)
        {

            //----------------------------------------------모니터 밝기조절----------------------------------------------
            // 밝기조절 텍스트 설정
            System.Windows.Forms.Label lblBrightness = new System.Windows.Forms.Label();
            lblBrightness.Text = "모니터 밝기 조절";
            lblBrightness.Location = new Point(20, 20);
            lblBrightness.Size = new Size(120, 20);

            // 밝기조절 트랙바 설정
            this.trackBarBrightness = new TrackBar();
            this.trackBarBrightness.Minimum = 0;
            this.trackBarBrightness.Maximum = 100;
            this.trackBarBrightness.TickStyle = TickStyle.None;
            this.trackBarBrightness.Location = new Point(150, 20);
            this.trackBarBrightness.ValueChanged += BrightnessTrackBar_ValueChanged;
            this.trackBarBrightness.Size = new Size(250, 45);
            this.trackBarBrightness.Text = "0";
            this.trackBarBrightness.TextChanged += BrightnessTextBox_TextChanged;
            
            // 밝기조절 텍스트박스 설정
            this.txtBrightnessValue = new TextBox();
            this.txtBrightnessValue.BorderStyle = BorderStyle.FixedSingle;
            this.txtBrightnessValue.Location = new Point(410, 20);
            this.txtBrightnessValue.MaxLength = 4;
            this.txtBrightnessValue.Size = new Size(65, 29);
            this.txtBrightnessValue.TabIndex = 2;
            this.txtBrightnessValue.TextAlign = HorizontalAlignment.Center;
            


            //----------------------------------------------모니터 감마조절----------------------------------------------
            this.sldValue = new TrackBar();
            this.tbValue = new TextBox();

            ((System.ComponentModel.ISupportInitialize)(this.sldValue)).BeginInit();
            this.SuspendLayout();

            // 감마조절 텍스트 설정
            System.Windows.Forms.Label lblGamma = new System.Windows.Forms.Label();
            lblGamma.Text = "모니터 감마 조절";
            lblGamma.Location = new Point(20, 80);
            lblGamma.Size = new Size(120, 20);

            // 감마조절 트랙바 설정
            this.sldValue.LargeChange = 10;
            this.sldValue.Location = new Point(150, 80);
            this.sldValue.Maximum = 4460;
            this.sldValue.Minimum = 228;
            this.sldValue.Size = new Size(250, 45);
            this.sldValue.TabIndex = 1;
            this.sldValue.TickStyle = TickStyle.None;
            this.sldValue.Value = 1000;
            this.sldValue.Scroll += new EventHandler(this.sldValue_Scroll);

            // 감마조절 텍스트박스 설정
            this.tbValue.BorderStyle = BorderStyle.FixedSingle;
            this.tbValue.Location = new Point(410, 80);
            this.tbValue.MaxLength = 4;
            this.tbValue.Size = new Size(65, 29);
            this.tbValue.TabIndex = 2;
            this.tbValue.TextAlign = HorizontalAlignment.Center;
            this.tbValue.TextChanged += new EventHandler(this.tbValue_TextChanged);
            this.tbValue.KeyPress += new KeyPressEventHandler(this.tbValue_KeyPress);
            this.tbValue.Enter += new EventHandler(this.tbValue_Enter);



            //----------------------------------------------모니터 회전조절----------------------------------------------

            // 모니터 회전 텍스트 설정
            System.Windows.Forms.Label lblOrientation = new System.Windows.Forms.Label();
            lblOrientation.Text = "모니터 화면 회전";
            lblOrientation.Location = new Point(20, 140);
            lblOrientation.Size = new Size(120, 20);

            // 모니터 회전 콤보박스 설정
            ComboBox comboOrientation = new ComboBox();
            comboOrientation.Location = new Point(150, 140);
            comboOrientation.Size = new Size(250, 20);
            comboOrientation.Items.AddRange(new string[] { "기본", "90도 회전", "180도 회전", "270도 회전" });
            comboOrientation.SelectedIndexChanged += (sender, e) =>
            {

                currentOrientation = (ScreenOrientation)comboOrientation.SelectedIndex;
                SetMonitorOrientation(currentOrientation);
            };
            comboOrientation.SelectedIndex = 0; // 초기값 설정



            //----------------------------------------------모니터 초기화 버튼----------------------------------------------

            this.resetMonitor = new Button();
            resetMonitor.Text = "모니터 설정 초기화";
            resetMonitor.Location = new Point(20, 200); // 위치 설정
            resetMonitor.Size = new Size(0, 0); // 크기 설정
            resetMonitor.AutoSize = true;
            resetMonitor.Click += new EventHandler(resetMonitor_Click);



            tabPageMonitor.Controls.Add(lblBrightness);
            tabPageMonitor.Controls.Add(lblOrientation);
            tabPageMonitor.Controls.Add(comboOrientation);
            tabPageMonitor.Controls.Add(trackBarBrightness);
            tabPageMonitor.Controls.Add(txtBrightnessValue);
            tabPageMonitor.Controls.Add(lblGamma);
            tabPageMonitor.Controls.Add(sldValue);
            tabPageMonitor.Controls.Add(tbValue);
            tabPageMonitor.Controls.Add(resetMonitor);
        }


        //----------------------------------------------모니터 감마설정----------------------------------------------
        private void sldValue_Scroll(object sender, EventArgs e)
        {
            this.tbValue.Text = this.sldValue.Value.ToString();
            this.currentGamma = (float)this.sldValue.Value / 1000f;
            this.gamma.Set(this.currentGamma);
        }

        private void tbValue_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.tbValue.Text))
                return;
            int num = int.Parse(this.tbValue.Text);
            if (num > this.sldValue.Maximum || num < this.sldValue.Minimum)
                return;
            this.sldValue.Value = num;
            this.sldValue_Scroll(sender, e);
        }

        private void tbValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || (int)e.KeyChar == (int)Convert.ToChar(Keys.Back))
                return;
            e.Handled = true;
        }

        private void tbValue_Enter(object sender, EventArgs e)
        {
            this.tbValue.SelectAll();
        }



        //----------------------------------------------모니터 밝기설정----------------------------------------------
        private void InitializeMonitor()
        {
            IntPtr hMonitor = MonitorFromWindow(this.Handle, 2);
            if (hMonitor != IntPtr.Zero)
            {
                uint monitorCount = 0;
                if (GetNumberOfPhysicalMonitorsFromHMONITOR(hMonitor, ref monitorCount))
                {
                    physicalMonitors = new PHYSICAL_MONITOR[monitorCount];
                    if (GetPhysicalMonitorsFromHMONITOR(hMonitor, monitorCount, physicalMonitors))
                    {
                        if (physicalMonitors != null && physicalMonitors.Length > 0)
                        {
                            if (GetMonitorBrightness(physicalMonitors[0].hPhysicalMonitor, out minBrightness, out currentBrightness, out maxBrightness))
                            {
                                trackBarBrightness.Value = (int)(currentBrightness * 100 / maxBrightness);
                                txtBrightnessValue.Text = trackBarBrightness.Value.ToString();
                            }
                        }
                    }
                }
            }
        }

        private void BrightnessTrackBar_ValueChanged(object sender, EventArgs e)
        {
            if (physicalMonitors != null && physicalMonitors.Length > 0)
            {
                uint newBrightness = (uint)(trackBarBrightness.Value * maxBrightness / 100);
                if (SetMonitorBrightness(physicalMonitors[0].hPhysicalMonitor, newBrightness))
                {
                    txtBrightnessValue.Text = trackBarBrightness.Value.ToString();
                }
            }
        }

        private void BrightnessTextBox_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(txtBrightnessValue.Text, out int brightness))
            {
                trackBarBrightness.Value = Math.Clamp(brightness, 0, 100);
                if (physicalMonitors != null && physicalMonitors.Length > 0)
                {
                    uint newBrightness = (uint)(trackBarBrightness.Value * maxBrightness / 100);
                    SetMonitorBrightness(physicalMonitors[0].hPhysicalMonitor, newBrightness);
                }
            }
        }

        //protected override void OnFormClosed(FormClosedEventArgs e)
        //{
        //    if (physicalMonitors != null && physicalMonitors.Length > 0)
        //    {
        //        DestroyPhysicalMonitors((uint)physicalMonitors.Length, physicalMonitors);
        //    }
        //    base.OnFormClosed(e);
        //}



        //----------------------------------------------모니터 회전설정----------------------------------------------
        private void SetMonitorOrientation(ScreenOrientation orientation)
        {
            DEVMODE dm = new DEVMODE();
            dm.dmSize = (ushort)Marshal.SizeOf(typeof(DEVMODE));

            // 현재 디스플레이 설정을 가져옵니다.
            if (EnumDisplaySettings(null, ENUM_CURRENT_SETTINGS, ref dm) != 0)
            {
                // 90도 또는 270도 회전일 경우, 너비와 높이를 교환합니다.
                if (orientation == ScreenOrientation.DMDO_90 || orientation == ScreenOrientation.DMDO_270)
                {
                    if (dm.dmDisplayOrientation == ScreenOrientation.DMDO_DEFAULT || dm.dmDisplayOrientation == ScreenOrientation.DMDO_180)
                    {
                        int temp = dm.dmPelsWidth;
                        dm.dmPelsWidth = dm.dmPelsHeight;
                        dm.dmPelsHeight = temp;
                    }
                }
                else
                {
                    if (dm.dmDisplayOrientation == ScreenOrientation.DMDO_90 || dm.dmDisplayOrientation == ScreenOrientation.DMDO_270)
                    {
                        int temp = dm.dmPelsWidth;
                        dm.dmPelsWidth = dm.dmPelsHeight;
                        dm.dmPelsHeight = temp;
                    }
                }

                dm.dmDisplayOrientation = orientation;
                dm.dmFields |= DM_DISPLAYORIENTATION | DM_PELSWIDTH | DM_PELSHEIGHT;

                int result = ChangeDisplaySettingsEx(null, ref dm, IntPtr.Zero, CDS_UPDATEREGISTRY | CDS_RESET, IntPtr.Zero);

                if (!isFirstOrientationChange)
                {
                    if (result != DISP_CHANGE_SUCCESSFUL)
                    {
                        MessageBox.Show("모니터 회전에 실패했습니다.");
                    }
                    else
                    {
                        //MessageBox.Show("모니터 회전이 성공했습니다.");
                    }
                }
                else
                {
                    isFirstOrientationChange = false; // 처음 변경 완료 후에는 초기화
                }
            }
            else
            {
                MessageBox.Show("Failed to retrieve display settings.");
            }
        }



        //----------------------------------------------모니터 초기화설정----------------------------------------------
        private void resetMonitor_Click(object sender, EventArgs e)
        {
            this.sldValue.Value = 1000;
            this.tbValue.Text = "1000";
            this.trackBarBrightness.Value = 50;
            SetMonitorOrientation(0);
        }



        //----------------------------------------------모니터 설정 저장----------------------------------------------
        private void SaveMonitorSettings(string filePath, string folderPath)
        {
            try
            {
                string fullPath = Path.Combine(Path.GetDirectoryName(filePath), folderPath);

                using (StreamWriter writer = new StreamWriter(Path.Combine(fullPath, Path.GetFileName(filePath))))
                {
                    writer.WriteLine($"trackBarBrightness={trackBarBrightness.Value}");
                    writer.WriteLine($"sldValue={sldValue.Value}");
                    writer.WriteLine($"currentOrientation={(int)currentOrientation}");
                    //writer.WriteLine($"ColorFilter={chkColorFilter.Checked}");
                }
                //MessageBox.Show("Monitor settings saved.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving monitor settings: {ex.Message}");
            }
        }












        // 오디오 컨트롤 탭에 관련한 초기화
        private TrackBar trackBarVolume;
        private System.Windows.Forms.Label labelVolume;
        private TrackBar trackBarMicVolume;
        private System.Windows.Forms.Label labelMicVolume;
        private TrackBar[] trackBars;
        private System.Windows.Forms.Label[] labels;
        private MMDeviceEnumerator devEnum;
        private MMDevice renderDevice;
        private MMDevice captureDevice;
        private BiQuadFilter[] filters;


        // 사운드 조절 함수
        private void InitializeUIComponents(TabPage tabPageAudio, TabControl tabControl)
        {

            this.trackBarVolume = new TrackBar();
            this.labelVolume = new System.Windows.Forms.Label();
            this.trackBarMicVolume = new TrackBar();
            this.labelMicVolume = new System.Windows.Forms.Label();
            this.trackBars = new TrackBar[10];
            this.labels = new System.Windows.Forms.Label[10];

            // 스피커 텍스트 설정
            System.Windows.Forms.Label lblVolume = new System.Windows.Forms.Label();
            lblVolume.Text = "스피커 음량";
            lblVolume.Location = new Point(20, 20);
            lblVolume.Size = new Size(120, 20);

            this.trackBarVolume.Location = new System.Drawing.Point(100, 20);
            this.trackBarVolume.Name = "trackBarVolume";
            this.trackBarVolume.Size = new System.Drawing.Size(250, 45);
            this.trackBarVolume.TabIndex = 0;
            this.trackBarVolume.TickStyle = TickStyle.None;

            this.labelVolume.AutoSize = true;
            this.labelVolume.Location = new System.Drawing.Point(360, 20);
            this.labelVolume.Name = "labelVolume";
            this.labelVolume.Size = new System.Drawing.Size(60, 30);
            this.labelVolume.TabIndex = 1;




            // 스피커 텍스트 설정
            System.Windows.Forms.Label lblMicVolume = new System.Windows.Forms.Label();
            lblMicVolume.Text = "마이크 음량";
            lblMicVolume.Location = new Point(20, 80);
            lblMicVolume.Size = new Size(120, 80);

            this.trackBarMicVolume.Location = new System.Drawing.Point(100, 80);
            this.trackBarMicVolume.Name = "trackBarMicVolume";
            this.trackBarMicVolume.Size = new System.Drawing.Size(260, 45);
            this.trackBarMicVolume.TabIndex = 2;
            this.trackBarMicVolume.TickStyle = TickStyle.None;

            this.labelMicVolume.AutoSize = true;
            this.labelMicVolume.Location = new System.Drawing.Point(360, 80);
            this.labelMicVolume.Name = "labelMicVolume";
            this.labelMicVolume.Size = new System.Drawing.Size(60, 30);
            this.labelMicVolume.TabIndex = 3;

            // 탭 페이지에 컨트롤 추가
            tabPageAudio.Controls.Add(lblVolume);
            tabPageAudio.Controls.Add(lblMicVolume);
            tabPageAudio.Controls.Add(this.trackBarVolume);
            tabPageAudio.Controls.Add(this.labelVolume);
            tabPageAudio.Controls.Add(this.trackBarMicVolume);
            tabPageAudio.Controls.Add(this.labelMicVolume);

        }

        private void InitializeVolumeControl()
        {
            devEnum = new MMDeviceEnumerator();
            renderDevice = devEnum.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

            // 트랙바 초기 설정
            trackBarVolume.Minimum = 0;
            trackBarVolume.Maximum = 100;
            trackBarVolume.Value = (int)(renderDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
            labelVolume.Text = $"{trackBarVolume.Value}%";

            // 트랙바 이벤트 핸들러 연결
            trackBarVolume.Scroll += TrackBarVolume_Scroll;
        }

        private void TrackBarVolume_Scroll(object sender, EventArgs e)
        {
            // 볼륨 설정
            float volume = trackBarVolume.Value / 100.0f;
            renderDevice.AudioEndpointVolume.MasterVolumeLevelScalar = volume;
            labelVolume.Text = $"{trackBarVolume.Value}%";
        }

        private void InitializeMicVolumeControl()
        {
            captureDevice = devEnum.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Multimedia);

            // 마이크 볼륨 트랙바 초기 설정
            trackBarMicVolume.Minimum = 0;
            trackBarMicVolume.Maximum = 100;
            trackBarMicVolume.Value = (int)(captureDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
            labelMicVolume.Text = $"{trackBarMicVolume.Value}%";

            // 트랙바 이벤트 핸들러 연결
            trackBarMicVolume.Scroll += TrackBarMicVolume_Scroll;
        }

        private void TrackBarMicVolume_Scroll(object sender, EventArgs e)
        {
            // 마이크 볼륨 설정
            float volume = trackBarMicVolume.Value / 100.0f;
            captureDevice.AudioEndpointVolume.MasterVolumeLevelScalar = volume;
            labelMicVolume.Text = $"{trackBarMicVolume.Value}%";
        }

        // 사운드 설정들 저장하는 함수
        private void SaveSoundSettings(string filePath, string folderPath)
        {
            try
            {
                string fullPath = Path.Combine(Path.GetDirectoryName(filePath), folderPath);

                using (StreamWriter writer = new StreamWriter(Path.Combine(fullPath, Path.GetFileName(filePath))))
                {
                    writer.WriteLine($"trackBarVolume={trackBarVolume.Value}");
                    writer.WriteLine($"trackBarMicVolume={trackBarMicVolume.Value}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving monitor settings: {ex.Message}");
            }
        }

    }


    // 커서를 숨기는 훅 이벤트 총괄 
    public static class HookManager
    {
        // Hooking constants
        private const int WH_MOUSE_LL = 14;

        // Mouse events
        public static event MouseEventHandler MouseMove = delegate { };
        public static event MouseEventHandler MouseClick = delegate { };
        public static event MouseEventHandler MouseDown = delegate { };
        public static event MouseEventHandler MouseUp = delegate { };

        private static LowLevelMouseProc _mouseProc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        public static void Start()
        {
            _hookID = SetHook(_mouseProc);
        }

        public static void Stop()
        {
            UnhookWindowsHookEx(_hookID);
        }

        private static IntPtr SetHook(LowLevelMouseProc proc)
        {
            using (var curProcess = System.Diagnostics.Process.GetCurrentProcess())
            using (var curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_MOUSE_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                var wmMouse = (MouseMessages)wParam;
                if (wmMouse == MouseMessages.WM_MOUSEMOVE)
                {
                    var hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                    MouseMove.Invoke(null, new MouseEventArgs(MouseButtons.None, 0, hookStruct.pt.x, hookStruct.pt.y, 0));
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private enum MouseMessages
        {
            WM_MOUSEMOVE = 0x0200,
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205,
            WM_MOUSEWHEEL = 0x020A,
            WM_MOUSEHWHEEL = 0x020E
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }
    }

    class DeviceSettings
    {
        // 마우스 감도 변경 함수
        [DllImport("user32.dll")]
        public static extern bool SystemParametersInfo(uint uiAction, uint uiParam, uint pvParam, uint fWinIni);

        const uint SPI_SETMOUSESPEED = 0x0071;

        public static void SetMouseSensitivity(int sensitivity)
        {
            SystemParametersInfo(SPI_SETMOUSESPEED, 0, (uint)sensitivity, 0);
        }

        // 모니터 밝기 및 대비 변경 함수 
        public static void SetMonitorSettings(int brightness, int contrast)
        {
            // 구현해야함
            Console.WriteLine($"Monitor Brightness: {brightness}, Contrast: {contrast}");
        }

        // 사운드 볼륨 및 음소거 변경 함수 
        public static void SetSoundSettings(int volume, bool mute)
        {
            // 구현해야함
            Console.WriteLine($"Sound Volume: {volume}, Mute: {mute}");
        }

        public static void ApplySettings(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Settings file not found.");
                return;
            }

            var lines = File.ReadAllLines(filePath);
            string currentSection = "";

            foreach (var line in lines)
            {
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    currentSection = line.Substring(1, line.Length - 2);
                }
                else
                {
                    var keyValue = line.Split('=');
                    if (keyValue.Length != 2) continue;

                    var key = keyValue[0].Trim();
                    var value = keyValue[1].Trim();

                    switch (currentSection)
                    {
                        case "Mouse":
                            if (key == "Sensitivity" && int.TryParse(value, out int sensitivity))
                            {
                                SetMouseSensitivity(sensitivity);
                            }
                            break;

                        case "Monitor":
                            if (key == "Brightness" && int.TryParse(value, out int brightness))
                            {
                                // 밝기 함수 호출
                                SetMonitorSettings(brightness, 0); // ex 밝기 0
                            }
                            else if (key == "Contrast" && int.TryParse(value, out int contrast))
                            {
                                // 대비 함수 호출
                                SetMonitorSettings(0, contrast); // ex 대비 0
                            }
                            break;

                        case "Sound":
                            if (key == "Volume" && int.TryParse(value, out int volume))
                            {
                                SetSoundSettings(volume, false); // ex 음소거
                            }
                            else if (key == "Mute" && bool.TryParse(value, out bool mute))
                            {
                                SetSoundSettings(0, mute); // ex 볼륨 0
                            }
                            break;
                    }
                }
            }
        }
    }


}