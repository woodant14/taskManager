using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Management;
using Microsoft.VisualBasic;
using System.Drawing.Text;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private List<Process> processList = null;
        private ListViewIC comparer = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void GetProcesses()
        {
            processList.Clear();
            processList = Process.GetProcesses().ToList<Process>();
        }
        private void RefreshProcessesList()
        {
            listView1.Items.Clear();
            double memSize = 0;
            foreach (var item in processList)
            {
                memSize = 0;

                PerformanceCounter pc = new PerformanceCounter();
                pc.CategoryName = "Process";
                pc.CounterName = "Working Set - Private";
                pc.InstanceName = item.ProcessName;
                memSize = (double)pc.NextValue() / (1000 * 1000);
                string[] row = new[] { item.ProcessName.ToString(), Math.Round(memSize, 1).ToString() };
                listView1.Items.Add(new ListViewItem(row));

                pc.Close();
                pc.Dispose();
            }
            Text = "Procces: " + processList.Count.ToString();
        }
        private void RefreshProcessesList(List<Process> processList, string key)
        {
            try
            {
                listView1.Items.Clear();
                double memSize = 0;
                foreach (var item in processList)
                {
                    if (item != null)
                    {
                        memSize = 0;

                        PerformanceCounter pc = new PerformanceCounter();
                        pc.CategoryName = "Process";
                        pc.CounterName = "Working Set - Private";
                        pc.InstanceName = item.ProcessName;
                        memSize = (double)pc.NextValue() / (1000 * 1000);
                        string[] row = new[] { item.ProcessName.ToString(), Math.Round(memSize, 1).ToString() };
                        listView1.Items.Add(new ListViewItem(row));

                        pc.Close();
                        pc.Dispose();
                    }
                }
            } catch (Exception) { }
            Text = "Procces found: " + processList.Count.ToString();
            
        }
        private void KillProc(Process proc)
        {
            proc.Kill();
            proc.WaitForExit();
        }
        private void KillAll( int id)
        {
            if(id == 0)
            {
                return;
            }
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                "Select * From Win32_Process Where ParentProcessID=" + id);
            ManagementObjectCollection objCollection = searcher.Get();
            foreach (ManagementObject obj in objCollection)
            {
                KillAll(Convert.ToInt32(obj["ProcessID"]));
            }
            try
            {
                Process p = Process.GetProcessById(id);
                p.Kill();
                p.WaitForExit();
            } catch (ArgumentException) { }
        }
        private int GetPID(Process process)
        {
            int pid = 0;
            try
            {
                ManagementObject managementObject = new ManagementObject("win32_process.handle='" + process.Id + "'");
                managementObject.Get();
                pid = Convert.ToInt32(managementObject["ParentProcessId"]);
            } catch (Exception) { }
            return pid;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            processList = new List<Process>();
            comparer = new ListViewIC();
            GetProcesses();
            RefreshProcessesList();
        }

        private void toolStripButton1_Click(object sender, EventArgs e) //Update ListView
        {
            GetProcesses();
            RefreshProcessesList();

        }

        private void toolStripButton2_Click(object sender, EventArgs e) //Kill One Process;
        {
            try
            {
                if (listView1.SelectedItems[0] != null)
                {
                    Process needProc = processList.Where((x) => x.ProcessName ==
                    listView1.SelectedItems[0].SubItems[0].Text).ToList()[0];
                    KillProc(needProc);

                    GetProcesses();
                    RefreshProcessesList();
                }
            } catch (Exception) { }
        }

        private void toolStripButton3_Click(object sender, EventArgs e) //Kill Parent Process
        {
            try
            {
                if (listView1.SelectedItems[0] != null)
                {
                    Process SelectedProc = processList.Where((x) => x.ProcessName ==
                    listView1.SelectedItems[0].SubItems[0].Text).ToList()[0]; //Select need Process;
                    KillAll(GetPID(SelectedProc));

                    GetProcesses();
                    RefreshProcessesList();
                }
            }
            catch (Exception) { }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void завершитьToolStripMenuItem_Click(object sender, EventArgs e) //Copy for ContextMenu
        {
            try
            {
                if (listView1.SelectedItems[0] != null)
                {
                    Process needProc = processList.Where((x) => x.ProcessName ==
                    listView1.SelectedItems[0].SubItems[0].Text).ToList()[0];
                    KillProc(needProc);

                    GetProcesses();
                    RefreshProcessesList();
                }
            }
            catch (Exception) { }
        }

        private void завершитьДеревоToolStripMenuItem_Click(object sender, EventArgs e) //Copy for ContextMenu
        {
            try
            {
                if (listView1.SelectedItems[0] != null)
                {
                    Process needProc = processList.Where((x) => x.ProcessName ==
                    listView1.SelectedItems[0].SubItems[0].Text).ToList()[0];
                    KillAll(GetPID(needProc));

                    GetProcesses();
                    RefreshProcessesList();
                }
            }
            catch (Exception) { }
        }

        private void addNewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = Interaction.InputBox("Enter programm name", "Start new process");
            try
            {
                Process.Start(path);   
            } catch (Exception) { } 
        }

        private void toolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            GetProcesses();
            List<Process> filtreProcess = processList.Where((x) => x.
          ProcessName.ToLower().Contains(toolStripTextBox1.Text.ToLower())).ToList<Process>();
            RefreshProcessesList(filtreProcess, toolStripTextBox1.Text);
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            comparer.ColumnIndex = e.Column;
            comparer.SortDirection = comparer.SortDirection == SortOrder.Ascending ? 
                SortOrder.Descending : SortOrder.Ascending;
            listView1.ListViewItemSorter = comparer;
            listView1.Sort();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
